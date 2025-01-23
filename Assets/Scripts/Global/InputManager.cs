namespace Scripts.Input
{
    public static class InputManager
    {
        public static Controls Controls { get; private set; }

        static InputManager()
        {
            Controls = new();
            Controls.Enable();
        }
    }
}
