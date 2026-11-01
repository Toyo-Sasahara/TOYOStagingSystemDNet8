namespace NetworkPrintService
{
    /// <summary>
    /// NetworkPrintService のホストプロセスを維持するバックグラウンドサービスです。
    /// </summary>
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        /// <summary>
        /// サービス起動中は無期限待機し、停止要求時に終了ログを出力します。
        /// </summary>
        /// <param name="stoppingToken">サービス停止要求を通知するキャンセルトークンです。</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("■ネットワークプリントサービス開始: {time}", DateTimeOffset.Now);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("■ネットワークプリントサービス停止: {time}", DateTimeOffset.Now);
            }
        }
    }
}
