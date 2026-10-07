// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions;

/// <summary>
/// 应用键（AppKey）形状校验器（跨产品线单点收敛：各产品线
/// <c>WechatAppConfigBase.Validate</c> 与各 <c>XxxAppManager</c> 共用同一实现）。
/// </summary>
/// <remarks>
/// <para>
/// 规则：首字符为字母或数字，其余仅 <c>[A-Za-z0-9._-]</c>，长度 ≤ 128（对齐组件 <c>AppKeyValidator</c>）。
/// </para>
/// <para>
/// <b>为何必须校验</b>：AppKey 参与两类外部标识的构造——
/// </para>
/// <list type="bullet">
/// <item>令牌持久化键：<c>{tokenType}:{appKey}:{scopeKey}</c>。含 <c>:</c> 会造成<b>键别名</b>，
/// 如 appKey = <c>"a:b"</c> 会与 <c>appKey = "a"</c> + <c>scopeKey = "b"</c> 的键重叠，导致跨应用令牌串号；</item>
/// <item>命名 HttpClient 名：<c>{前缀}-{appKey}</c>。含 <c>/</c>、<c>..</c>、空格或控制字符会污染
/// 客户端名（并可能被宿主用于日志 / 诊断注入）。</item>
/// </list>
/// </remarks>
public static class WechatAppKeyValidator
{
    /// <summary>AppKey 最大长度。</summary>
    public const int MaxLength = 128;

    /// <summary>
    /// 校验 AppKey 形状；不合法时抛出 <see cref="InvalidOperationException"/>（启动期快速失败）。
    /// </summary>
    /// <param name="appKey">待校验的应用键。</param>
    /// <exception cref="InvalidOperationException">形状非法时抛出。</exception>
    public static void Validate(string appKey)
    {
        // 空值必须与 IsValid("") 的判定同源（否则两个入口会给出相反结论：
        // Validate 放行空键、IsValid 判非法）。产品线通常在调用前先判空白并给出更具体的消息，
        // 此处作为契约兜底。
        if (string.IsNullOrEmpty(appKey))
        {
            throw new InvalidOperationException("AppKey 不能为空。");
        }

        if (appKey.Length > MaxLength)
        {
            throw new InvalidOperationException(
                $"应用 {AppKeyText(appKey)} 的 AppKey 长度不得超过 {MaxLength}（当前 {appKey.Length}）。");
        }

        for (var i = 0; i < appKey.Length; i++)
        {
            var c = appKey[i];
            var isAsciiLetter = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
            var isDigit = c >= '0' && c <= '9';
            var isSeparator = c == '.' || c == '_' || c == '-';
            var legal = i == 0 ? isAsciiLetter || isDigit : isAsciiLetter || isDigit || isSeparator;
            if (!legal)
            {
                throw new InvalidOperationException(
                    $"应用 {AppKeyText(appKey)} 的 AppKey 只能由字母、数字、'.'、'_'、'-' 组成，且首字符必须是字母或数字" +
                    "（AppKey 参与令牌持久化键与命名 HttpClient 名，禁止 ':'、'/'、空格等字符）。");
            }
        }
    }

    /// <summary>
    /// 判定 AppKey 形状是否合法（<b>不抛异常</b>的只读变体；供需要「先判定再决策」的调用点使用，
    /// 避免以异常做流程控制）。
    /// </summary>
    /// <param name="appKey">待判定的应用键。</param>
    /// <returns>合法返回 <c>true</c>。</returns>
    public static bool IsValid(string? appKey)
    {
        if (string.IsNullOrEmpty(appKey) || appKey!.Length > MaxLength)
        {
            return false;
        }

        for (var i = 0; i < appKey.Length; i++)
        {
            var c = appKey[i];
            var isAsciiLetter = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
            var isDigit = c >= '0' && c <= '9';
            var isSeparator = c == '.' || c == '_' || c == '-';
            if (i == 0 ? !(isAsciiLetter || isDigit) : !(isAsciiLetter || isDigit || isSeparator))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>异常消息中的 AppKey 展示文本（截断，避免超长键污染日志）。</summary>
    private static string AppKeyText(string appKey)
        => appKey.Length <= 32 ? appKey : appKey.Substring(0, 32) + "...";
}
