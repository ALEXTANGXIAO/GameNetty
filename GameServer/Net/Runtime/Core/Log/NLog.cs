#if GAMESERVER_NET
using NLog;

namespace GameServer;

/// <summary>
/// 使用 NLog 实现的日志记录器。
/// </summary>
public class NLog : ILog
{
    private readonly Logger _logger; // NLog 日志记录器实例

    /// <summary>
    /// 初始化 NLog 实例。
    /// </summary>
    /// <param name="name">日志记录器的名称。</param>
    public NLog(string name)
    {
        // 获取指定名称的 NLog 日志记录器
        _logger = LogManager.GetLogger(name);
    }

    /// <summary>
    /// 记录跟踪级别的日志消息。
    /// </summary>
    /// <param name="message">日志消息。</param>
    public void Trace(string message)
    {
        string result = $"[TRACE] - {message}";
        _logger.Trace(result);
    }

    /// <summary>
    /// 记录警告级别的日志消息。
    /// </summary>
    /// <param name="message">日志消息。</param>
    public void Warning(string message)
    {
        string result = $"[WARN ] - {message}";
        _logger.Warn(result);
    }

    /// <summary>
    /// 记录信息级别的日志消息。
    /// </summary>
    /// <param name="message">日志消息。</param>
    public void Info(string message)
    {
        string result = $"[INFO ] - {message}";
        _logger.Info(result);
    }

    /// <summary>
    /// 记录调试级别的日志消息。
    /// </summary>
    /// <param name="message">日志消息。</param>
    public void Debug(string message)
    {
        string result = $"[DEBUG] - {message}";
        _logger.Debug(result);
    }

    /// <summary>
    /// 记录错误级别的日志消息。
    /// </summary>
    /// <param name="message">日志消息。</param>
    public void Error(string message)
    {
        string result = $"[ERROR] - {message}";
        _logger.Error(result);
    }

    /// <summary>
    /// 记录严重错误级别的日志消息。
    /// </summary>
    /// <param name="message">日志消息。</param>
    public void Fatal(string message)
    {
        string result = $"[FATAL] - {message}";
        _logger.Fatal(result);
    }

    /// <summary>
    /// 记录跟踪级别的格式化日志消息。
    /// </summary>
    /// <param name="message">日志消息模板。</param>
    /// <param name="args">格式化参数。</param>
    public void Trace(string message, params object[] args)
    {
        string messageFormat = string.Format(message, args);
        string result = $"[TRACE] - {messageFormat}";
        _logger.Trace(result);
    }

    /// <summary>
    /// 记录警告级别的格式化日志消息。
    /// </summary>
    /// <param name="message">日志消息模板。</param>
    /// <param name="args">格式化参数。</param>
    public void Warning(string message, params object[] args)
    {
        string messageFormat = string.Format(message, args);
        string result = $"[WARN ] - {messageFormat}";
        _logger.Warn(result);
    }

    /// <summary>
    /// 记录信息级别的格式化日志消息。
    /// </summary>
    /// <param name="message">日志消息模板。</param>
    /// <param name="args">格式化参数。</param>
    public void Info(string message, params object[] args)
    {
        string messageFormat = string.Format(message, args);
        string result = $"[INFO ] - {messageFormat}";
        _logger.Info(result);
    }

    /// <summary>
    /// 记录调试级别的格式化日志消息。
    /// </summary>
    /// <param name="message">日志消息模板。</param>
    /// <param name="args">格式化参数。</param>
    public void Debug(string message, params object[] args)
    {
        string messageFormat = string.Format(message, args);
        string result = $"[DEBUG] - {messageFormat}";
        _logger.Debug(result);
    }

    /// <summary>
    /// 记录错误级别的格式化日志消息。
    /// </summary>
    /// <param name="message">日志消息模板。</param>
    /// <param name="args">格式化参数。</param>
    public void Error(string message, params object[] args)
    {
        string messageFormat = string.Format(message, args);
        string result = $"[ERROR] - {messageFormat}";
        _logger.Error(result);
    }

    /// <summary>
    /// 记录严重错误级别的格式化日志消息。
    /// </summary>
    /// <param name="message">日志消息模板。</param>
    /// <param name="args">格式化参数。</param>
    public void Fatal(string message, params object[] args)
    {
        string messageFormat = string.Format(message, args);
        string result = $"[FATAL] - {messageFormat}";
        _logger.Fatal(result);
    }
}
#endif