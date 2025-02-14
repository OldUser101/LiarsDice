namespace LiarsDice
{
    public class Menu(IEnumerable<string>? options, Action? initialAction, Action? finalAction)
    {
        private IEnumerable<string>? items = options;
        private Action? initialAction = initialAction;
        private Action? finalAction = finalAction;

        public void SetOptions(IEnumerable<string>? options)
        {
            this.items = options;
        }

        public void SetInitialAction(Action? initialAction)
        {
            this.initialAction = initialAction;
        }

        public void SetFinalAction(Action? finalAction)
        {
            this.finalAction = finalAction;
        }

        private void DisplayMenu(int cursorPos)
        {
            if (this.items is null)
            {
                return;
            }

            for (int i = 0; i < this.items.Count(); i++)
            {
                Console.WriteLine($"{(i == cursorPos ? '>' : ' ')} {this.items.ElementAt(i)}");
            }
        }

        public int ShowMenu()
        {
            int selectedItem = 0;
            bool exit = false;
            bool cancel = false;

            if (this.items is not null && this.items.Any())
            {
                Console.CursorVisible = false;

                while (!exit && !cancel)
                {
                    Console.Clear();

                    if (this.initialAction is not null)
                    {
                        this.initialAction();
                    }

                    DisplayMenu(selectedItem);
                    ConsoleKeyInfo key = Console.ReadKey(intercept: true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Escape:
                            cancel = true;
                            break;
                        case ConsoleKey.Enter:
                            exit = true;
                            break;
                        case ConsoleKey.UpArrow:
                            selectedItem--;
                            if (selectedItem < 0)
                            {
                                selectedItem = this.items.Count() - 1;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            selectedItem++;
                            if (selectedItem >= this.items.Count())
                            {
                                selectedItem = 0;
                            }
                            break;
                    }
                }

                Console.CursorVisible = true;
            }

            if (this.finalAction is not null)
            {
                this.finalAction();
            }

            if (exit && !cancel)
            {
                return selectedItem;
            }

            return -1;
        }
    }
}
