using Interpreter.Lib.VM;

namespace Interpreter.Lib.IR.Instructions
{
    public class PushParameterInstruction : Instruction
    {
        private readonly string _parameter;
        private readonly IValue _value;

        public PushParameterInstruction(
            int number,
            string parameter,
            IValue value
        ) : base(number)
        {
            _parameter = parameter;
            _value = value;
        }

        public override int Execute(VirtualMachine vm)
        {
            vm.Arguments.Push((_parameter, _value.Get(vm.Frames.Peek())));
            return Number + 1;
        }

        protected override string ToStringRepresentation() => $"PushParameter {_parameter} = {_value}";
    }
}