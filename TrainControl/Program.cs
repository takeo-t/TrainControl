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

using (MotorController motorController = new MotorController())
using (Sensor Sensor = new Sensor())
{
    motorController.MaximumDutyCycle = maxDutyCycle;

    Console.WriteLine("走行開始");

    // モーターを前進しながら、センサーを並行して監視する
    var motorTask = motorController.GoBackward(5000);  // モーターを後退
    var sensorTask = motorController.MonitorSensorAndStop(sensor: Sensor);  // センサーを監視して停止

    // 並行して実行されたタスクのどちらかが完了するまで待つ
    await Task.WhenAny(motorTask, sensorTask);

    // モーターの前進が終了、もしくはセンサーが検知して停止したら終了
    Console.WriteLine("走行終了 Ctrl+Cキーを押すとプログラムを終了します。");
}

Console.ReadLine();
