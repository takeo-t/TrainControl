namespace TrainControl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enterキーを押してアプリケーションを起動します。");
            Console.ReadLine();

            Console.Write("最高速度を入力してください（デューティー比 0.0 ～ 1.0）: ");
            double maxDutyCycle;
            while(!double.TryParse(Console.ReadLine(), out maxDutyCycle) || maxDutyCycle < 0 || maxDutyCycle > 1)
            {
                Console.WriteLine("無効な値です。0.0 ～ 1.0 の間で入力してください。");
            }

            using (MotorController motorController = new MotorController())
            {
                motorController.MaximumDutyCycle = maxDutyCycle;

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
