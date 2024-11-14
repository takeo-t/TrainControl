using AdcTutorial;
using TrainControl;

Console.WriteLine("Enterキーを押してアプリケーションを起動します。");
Console.ReadLine();

Console.Write("最高速度を入力してください（デューティー比 0.0 ～ 1.0）: ");
double maxDutyCycle;
while (!double.TryParse(Console.ReadLine(), out maxDutyCycle) || maxDutyCycle < 0 || maxDutyCycle > 1)
{
    Console.WriteLine("無効な値です。0.0 ～ 1.0 の間で入力してください。");
}

using (Sensor sensor = new Sensor())
using (MotorController motorController = new MotorController(sensor))
{
    motorController.MaximumDutyCycle = maxDutyCycle;

    Console.WriteLine("走行開始");

    for (int i = 0; i < 5; i++)
    {
        // モーターを前進しながら、センサーを並行して監視する
        var motorTask = motorController.GoForward(5000);  // モーターを前進
        var sensorTask = motorController.MonitorSensorAndStop(sensor: sensor, sensorNumber: 0);  // センサー0を監視して停止

        // 並行して実行されたタスクのどちらかが完了するまで待つ
        var completedTask = await Task.WhenAny(motorTask, sensorTask);

        // センサーの監視タスクが完了した場合にのみ、GoBackwardメソッドを実行
        if (completedTask == sensorTask)
        {
            var motorTask2 = motorController.GoBackward(5000);  // モーターを後退
            var sensorTask2 = motorController.MonitorSensorAndStop(sensor: sensor, sensorNumber: 1);  // センサー1を監視して停止

            // 並行して実行されたタスクのどちらかが完了するまで待つ
            completedTask = await Task.WhenAny(motorTask2, sensorTask2);

            // センサーの監視タスクが完了した場合にのみ、次の前進を実行
            if (completedTask == sensorTask2)
            {
                continue;
            }
        }
    }

    // モーターの前進が終了、もしくはセンサーが検知して停止したら終了
    Console.WriteLine("走行終了 Ctrl+Cキーを押すとプログラムを終了します。");
}

Console.ReadLine();