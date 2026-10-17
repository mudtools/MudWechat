// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 工具入参里的时间文本解析（多个域共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只认两种形态</b>：Unix 秒（<c>"1700000000"</c>）或<b>带时区</b>的 ISO 8601
/// （<c>"2026-10-10T09:00:00+08:00"</c> / <c>"2026-10-10T01:00:00Z"</c>）。
/// </para>
/// <para>
/// <b>无时区一律拒绝</b>：<see cref="DateTimeOffset.TryParse(string, out DateTimeOffset)"/> 对无偏移文本会按
/// <b>宿主本地时区</b>解释，同一段对话在不同地域的实例上会得到不同秒数——这种「静默漂移」比直接报错更难查，
/// 故判为非法并回填格式提示。
/// </para>
/// <para>
/// 官方各端点的跨度上限不同（如会议列表 180 天、参会人 31 天、打卡 30 天），跨度校验留在各执行器里做，
/// 本类只负责「文本 → 秒」这一件事。
/// </para>
/// </remarks>
internal static class ToolTimeText
{
    /// <summary>解析时间入参。</summary>
    /// <param name="raw">模型侧原始文本（Unix 秒或带时区的 ISO 8601）。</param>
    /// <param name="seconds">解析出的 Unix 秒（失败为 0）。</param>
    /// <returns>是否解析成功。</returns>
    internal static bool TryParseUnixSeconds(string? raw, out long seconds)
    {
        seconds = 0;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var text = raw.Trim();
        if (long.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var numeric))
        {
            if (numeric <= 0)
            {
                return false;
            }

            seconds = numeric;
            return true;
        }

        // 日期段的 '-' 在 'T' 之前；'T' 之后出现的 '+/-' 或末尾的 'Z' 才是时区偏移。
        var tIndex = text.IndexOf('T');
        var hasOffset = text.EndsWith("Z", StringComparison.OrdinalIgnoreCase)
            || (tIndex >= 0 && (text.IndexOf('+', tIndex) >= 0 || text.LastIndexOf('-') > tIndex));
        if (!hasOffset
            || !DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var when))
        {
            return false;
        }

        seconds = when.ToUnixTimeSeconds();
        return true;
    }
}
