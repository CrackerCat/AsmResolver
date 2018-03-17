﻿using System;
using System.Collections.Generic;
using AsmResolver.Net.Cil;
using AsmResolver.Net.Cts;
using AsmResolver.Net.Signatures;

namespace AsmResolver.Net.Metadata
{
    public class MemberCloner
    {
        private sealed class MemberClonerReferenceImporter : ReferenceImporter
        {
            private readonly MemberCloner _memberCloner;

            public MemberClonerReferenceImporter(MemberCloner memberCloner, MetadataImage image) 
                : base(image)
            {
                _memberCloner = memberCloner;
            }

            public override ITypeDefOrRef ImportType(TypeDefinition type)
            {
                IMemberReference newType;
                return _memberCloner._createdMembers.TryGetValue(type, out newType)
                    ? (ITypeDefOrRef) newType
                    : base.ImportType(type);
            }

            public override IMethodDefOrRef ImportMethod(MethodDefinition method)
            {
                IMemberReference newMember;
                return _memberCloner._createdMembers.TryGetValue(method, out newMember)
                    ? (IMethodDefOrRef) newMember
                    : base.ImportMethod(method);
            }

            public override IMemberReference ImportField(FieldDefinition field)
            {
                IMemberReference newMember;
                return _memberCloner._createdMembers.TryGetValue(field, out newMember)
                    ? newMember
                    : base.ImportField(field);
            }
        }

        private readonly MetadataImage _targetImage;
        private readonly IDictionary<IMemberReference, IMemberReference> _createdMembers =
            new Dictionary<IMemberReference, IMemberReference>();
        private readonly IReferenceImporter _importer;

        public MemberCloner(MetadataImage targetImage)
        {
            if (targetImage == null)
                throw new ArgumentNullException("targetImage");
            _targetImage = targetImage;
            _importer = new MemberClonerReferenceImporter(this, targetImage);
        }

        public TypeDefinition CloneType(TypeDefinition type)
        {
            var newType = CreateTypeStub(type);
            DeclareMemberStubs(type, newType);
            FinalizeTypeStub(type, newType);
            return newType;
        }

        private FieldDefinition CloneField(FieldDefinition field)
        {
            var newField = new FieldDefinition(field.Name, field.Attributes, _importer.ImportFieldSignature(field.Signature));

            if (newField.Constant != null)
                newField.Constant = new Constant(field.Constant.ConstantType, new DataBlobSignature(field.Constant.Value.Data));
            
            _createdMembers.Add(field, newField);
            
            return newField;
        }

        private TypeDefinition CreateTypeStub(TypeDefinition type)
        {
            var newType = new TypeDefinition(type.Namespace, type.Name, type.Attributes);
            _createdMembers.Add(type, newType);
            
            foreach (var nestedType in type.NestedClasses)
                newType.NestedClasses.Add(new NestedClass(CreateTypeStub(nestedType.Class)));

            return newType;
        }

        private void DeclareMemberStubs(TypeDefinition type, TypeDefinition stub)
        {
            foreach (var nestedType in type.NestedClasses)
                DeclareMemberStubs(nestedType.Class, (TypeDefinition) _createdMembers[nestedType.Class]);
            
            foreach (var field in type.Fields)
                stub.Fields.Add(CloneField(field));
            foreach (var method in type.Methods)
                stub.Methods.Add(CreateMethodStub(method));
        }

        private void FinalizeTypeStub(TypeDefinition type, TypeDefinition stub)
        {
            foreach (var nestedType in type.NestedClasses)
                FinalizeTypeStub(nestedType.Class, (TypeDefinition) _createdMembers[nestedType.Class]);
            
            stub.BaseType = _importer.ImportType(type.BaseType);

            foreach (var method in type.Methods)
                FinalizeMethod(method, (MethodDefinition) _createdMembers[method]);
        }
        
        private MethodDefinition CreateMethodStub(MethodDefinition method)
        {
            var newMethod = new MethodDefinition(
                method.Name,
                method.Attributes,
                _importer.ImportMethodSignature(method.Signature));

            foreach (var parameter in method.Parameters)
                newMethod.Parameters.Add(new ParameterDefinition(parameter.Sequence, parameter.Name, parameter.Attributes));
            
            _createdMembers.Add(method, newMethod);
            return newMethod;
        }

        private void FinalizeMethod(MethodDefinition method, MethodDefinition stub)
        {
            if (method.CilMethodBody != null)
                stub.CilMethodBody = CloneCilMethodBody(method.CilMethodBody, stub);
        }

        private CilMethodBody CloneCilMethodBody(CilMethodBody body, MethodDefinition newOwner)
        {
            var newBody = new CilMethodBody(newOwner)
            {
                InitLocals = body.InitLocals,
                MaxStack = body.MaxStack,
            };

            if (body.Signature != null)
                newBody.Signature = _importer.ImportStandAloneSignature(body.Signature);

            CloneInstructions(body, newBody);
            CloneExceptionHandlers(body, newBody);

            return newBody;
        }

        private void CloneInstructions(CilMethodBody body, CilMethodBody newBody)
        {
            var branchInstructions = new List<CilInstruction>();
            var switchInstructions = new List<CilInstruction>();
            foreach (var instruction in body.Instructions)
            {
                object operand = instruction.Operand;
                if (operand is IMemberReference)
                    operand = _importer.ImportReference((IMemberReference) operand);
                
                var newInstruction = new CilInstruction(instruction.Offset, instruction.OpCode, operand);
                newBody.Instructions.Add(newInstruction);
                switch (instruction.OpCode.OperandType)
                {
                    case CilOperandType.InlineBrTarget:
                    case CilOperandType.ShortInlineBrTarget:
                        branchInstructions.Add(newInstruction);
                        break;
                    case CilOperandType.InlineSwitch:
                        switchInstructions.Add(newInstruction);
                        break;
                }
            }

            foreach (var branch in branchInstructions)
                branch.Operand = newBody.GetInstructionByOffset(((CilInstruction) branch.Operand).Offset);

            foreach (var @switch in switchInstructions)
            {
                var targets = (IEnumerable<CilInstruction>) @switch.Operand;
                var newTargets = new List<CilInstruction>();
                foreach (var target in targets)
                    newTargets.Add(newBody.GetInstructionByOffset(target.Offset));
                @switch.Operand = newTargets;
            }
        }

        private void CloneExceptionHandlers(CilMethodBody body, CilMethodBody newBody)
        {
            foreach (var handler in body.ExceptionHandlers)
            {
                var newHandler = new ExceptionHandler(handler.HandlerType)
                {
                    TryStart = newBody.GetInstructionByOffset(handler.TryStart.Offset),
                    TryEnd = newBody.GetInstructionByOffset(handler.TryEnd.Offset),
                    HandlerStart = newBody.GetInstructionByOffset(handler.HandlerStart.Offset),
                    HandlerEnd = newBody.GetInstructionByOffset(handler.HandlerEnd.Offset),
                };
                
                if (handler.FilterStart != null)
                    newHandler.FilterStart = newBody.GetInstructionByOffset(handler.FilterStart.Offset);
                if (handler.CatchType != null)
                    newHandler.CatchType = _importer.ImportType(handler.CatchType);
                
                newBody.ExceptionHandlers.Add(newHandler);
            }
        }
    }
}