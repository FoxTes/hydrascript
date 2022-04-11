using System.Collections.Generic;
using Interpreter.Lib.VM;

namespace Interpreter.Lib.IR.Instructions
{
    public class CallInstruction : ThreeAddressCodeInstruction
    {
        private readonly FunctionInfo _function;
        private readonly int _numberOfArguments;
        
        public CallInstruction(FunctionInfo function, int number, int numberOfArguments, string left = null) :
            base(left, (null, null), "Call ", number)
        {
            _function = function;
            _numberOfArguments = numberOfArguments;
        }

        public override int Jump() => _function.Location;

        public override int Execute(VirtualMachine vm)
        {
            var frame = new Frame(Number + 1, vm.Frames.Peek());

            var i = 0;
            var args = new List<(string Id, object Value)>();
            while (i < _numberOfArguments)
            {
                args.Add(vm.Arguments.Pop());
                frame[args[i].Id] = args[i].Value;
                i++;
            }

            vm.CallStack.Push(new Call(Number, _function, args, Left));
            vm.Frames.Push(frame);
            return _function.Location;
        }

        protected override string ToStringRepresentation() => Left == null
            ? $"Call {_function}, {_numberOfArguments}"
            : $"{Left} = Call {_function}, {_numberOfArguments}";
    }
}