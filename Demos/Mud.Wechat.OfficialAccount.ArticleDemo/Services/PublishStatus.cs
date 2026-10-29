// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Services;

/// <summary>
/// 官方 <c>publish_status</c>（标量 0~6）的语义与终态判定（方案 §5.3）。
/// <para>
/// <b>UI 口径</b>：数字与中文释义<b>并列原样显示</b>，0 与 5/6 不得合并 ——
/// 「发布成功」和「成功后被用户删除 / 被系统封禁」是三件不同的事，糊成「失败」会误导读者。
/// </para>
/// </summary>
internal static class PublishStatus
{
    public const int Success = 0;
    public const int Publishing = 1;
    public const int OriginalFailed = 2;
    public const int NormalFailed = 3;
    public const int AuditRejected = 4;
    public const int DeletedByUser = 5;
    public const int BlockedBySystem = 6;

    /// <summary>除「发布中」外的已知状态都是终态（官方不会再变）。</summary>
    public static bool IsTerminal(int? status) => status.HasValue && status.Value != Publishing;

    public static string Describe(int? status) => status switch
    {
        null => "未知（尚未轮询到状态）",
        Success => "发布成功",
        Publishing => "发布中",
        OriginalFailed => "原创失败",
        NormalFailed => "常规失败",
        AuditRejected => "平台审核不通过",
        DeletedByUser => "成功后被用户删除",
        BlockedBySystem => "成功后被系统封禁",
        _ => $"官方未定义的状态值 {status.Value}",
    };

    /// <summary>
    /// 官方 <c>fail_idx</c> 数组 → 落库文本（方案 §7.3：存 JSON 文本而非 primitive collection，
    /// 减少 Provider 行为差异，读取时手工反序列化）。
    /// </summary>
    public static string? ToFailIdxJson(System.Collections.Generic.List<long>? failIdx)
        => failIdx is { Count: > 0 } ? System.Text.Json.JsonSerializer.Serialize(failIdx) : null;
}
