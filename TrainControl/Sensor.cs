using Iot.Device.Adc;
using System;
using System.Device.Spi;
using System.Threading;

namespace AdcTutorial
{
    public class Sensor : IDisposable
    {
        private bool disposed = false;
        private readonly SpiDevice spi;
        private readonly Mcp3008 mcp;

        public Sensor()
        {
            // SPI接続の設定
            var hardwareSpiSettings = new SpiConnectionSettings(0, 0);
            spi = SpiDevice.Create(hardwareSpiSettings);
            mcp = new Mcp3008(spi);
        }

        /// <summary>
        /// センサーの値を取得
        /// </summary>
        /// <returns></returns>
        public double GetSensorValue()
        {
            return mcp.Read(0);
        }

        // センサーから値を読み取るメソッド
        //public async Task ReadSensorData()
        //{
        //    while (true)
        //    {
        //        Console.Clear();
        //        double value = mcp.Read(0);
        //        Console.WriteLine($"{value}");
        //        Console.WriteLine($"{Math.Round(value / 10.23, 1)}%");
        //        await Task.Delay(500);
        //    }
        //}

        // リソースの解放
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    mcp?.Dispose();
                    spi?.Dispose();
                }
                disposed = true;
            }
        }

        // IDisposableの実装
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

