using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainControl
{
    class Program
    {
        static void Main(string[] args)
        {
            // アタッチする時間を確保するためにユーザー入力を待機
            Console.WriteLine("Enterキーを押してアプリケーションを起動します。");
            Console.ReadLine();

            MotorController motorController = new MotorController();

            Console.WriteLine("Testing motor controller...");

            // モーターを前進させるテスト
            motorController.GoForward(5000); // 5秒間前進

            // モーターを後退させるテスト
            motorController.GoBackward(5000); // 5秒間後退

            Console.WriteLine("Test completed. Exiting program.");
        }
    }
}
