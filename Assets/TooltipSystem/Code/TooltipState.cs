namespace TooltipSystem.Code.PopupState
{
    public static class TooltipState
    {
        public record TooltipStateBase;

        public sealed record Preparing(float ShowAt) : TooltipStateBase;

        public sealed record Shown(float MakePreFixedAt) : TooltipStateBase;

        public sealed record PreFixed : TooltipStateBase;

        public sealed record Fixed : TooltipStateBase;

        public static TooltipStateBase CreatePreparing(float showAt) => new Preparing(showAt);

        public static TooltipStateBase CreateShown(float makePreFixedAt) => new Shown(makePreFixedAt);

        public static TooltipStateBase CreatePreFixed() => new PreFixed();

        public static TooltipStateBase CreateFixed() => new Fixed();
    }
}