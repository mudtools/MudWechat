// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 接口调用许可「自动激活回调通知」事件（<c>auto_activate</c>，官方 path 97198）的单个许可账号项
/// （官方 <c>AccountList</c>，根下重复同名兄弟元素、无包装容器）。
/// </summary>
/// <remarks>
/// 经上游 G-ADR-17 的 <c>[PayloadContract]</c> 生成字段映射；元素定位与「单/多形态分派」由
/// <c>WechatPayloadConverter.RepeatLicenseAccountItems</c> 承担（依赖根层同名兄弟合并投影，
/// 与 <c>UploadInfo</c>/<c>ChangeList</c> 同形态）。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackLicenseAccountItem
{
    /// <summary>自动激活的许可账号激活码（官方 <c>ActiveCode</c>）。</summary>
    [PayloadField("ActiveCode")]
    public string? ActiveCode { get; set; }

    /// <summary>许可类型（官方 <c>Type</c>：1 基础许可 / 2 互通许可）。</summary>
    [PayloadField("Type")]
    public long? Type { get; set; }

    /// <summary>自动激活后该许可的到期时间（官方 <c>ExpireTime</c>，Unix 秒）。</summary>
    [PayloadField("ExpireTime")]
    public long? ExpireTime { get; set; }

    /// <summary>自动激活成员的 UserID（官方 <c>UserId</c>）。</summary>
    [PayloadField("UserId")]
    public string? UserId { get; set; }

    /// <summary>
    /// 激活前许可状态（官方 <c>PreviousStatus</c>：1 未激活 / 2 已激活且未过期（剩余时长 ≤ 7 天）/
    /// 3 已激活且已过期）。
    /// </summary>
    [PayloadField("PreviousStatus")]
    public long? PreviousStatus { get; set; }

    /// <summary>
    /// 该成员的旧激活码（官方 <c>PreviousActiveCode</c>）；
    /// <b>仅当对已激活成员进行自动激活时返回</b>，其余情况缺失。
    /// </summary>
    [PayloadField("PreviousActiveCode")]
    public string? PreviousActiveCode { get; set; }
}
