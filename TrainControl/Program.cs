namespace TrainControl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enterキーを押してアプリケーションを起動します。");
            Console.ReadLine();

            using (MotorController motorController = new MotorController())
            {
                Console.WriteLine("走行開始");

                // モーターを前進
                await motorController.GoForward(5000); // 5秒間前進

                // モーターを後退
                await motorController.GoBackward(5000); // 5秒間後退

                Console.WriteLine("走行終了 Ctrl+Cキーを押すとプログラムを終了します。");
            }

            Console.ReadLine();
        }
    }
}
