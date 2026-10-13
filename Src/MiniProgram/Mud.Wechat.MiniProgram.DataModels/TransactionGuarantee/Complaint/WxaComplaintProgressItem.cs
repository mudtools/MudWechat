// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 投诉进度节点（<c>item</c>，官方参数表为<b>单数对象</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b><see cref="BlameResult"/> 的官方语义（照录）</b>：仅当 <see cref="ItemType"/> 为 <c>31</c> / <c>32</c>
/// （平台已核实此投诉为商家责任）时用于判断是否经过退货描述——<c>1</c> 对应「待用户退货中」，
/// <c>0</c> 对应「待上传处理凭证」⇒ 该字段决定商家侧应走 <c>bussiSupplyRefund</c>（退货）
/// 还是 <c>bussiSupplyProof</c>（补充凭证）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaComplaintProgressItem
{
    /// <summary>
    /// 投诉节点状态（<c>itemType</c>；官方枚举关键取值：<c>1</c> 用户发起投诉、<c>2</c>/<c>3</c> 用户/商家补充留言、
    /// <c>7</c>/<c>8</c> 用户/商家补充凭证、<c>12</c> 用户撤销投诉、<c>16</c> 待商家补充凭证、
    /// <c>30</c>/<c>33</c> 平台核实非商责已完结、<c>31</c>/<c>32</c> 平台核实为商责（配合 <see cref="BlameResult"/>）、
    /// <c>36</c> 平台已核实处理凭证投诉完结、<c>109</c> 商家已回应投诉、<c>110</c> 商家提交投诉处理凭证、
    /// <c>118</c> 商责自动赔付，以及各超时分支）。
    /// </summary>
    [JsonPropertyName("itemType")]
    public long? ItemType { get; set; }

    /// <summary>时间（<c>time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>内容文本（<c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>图片 cdn 资源列表（<c>mediaIdList</c>，官方类型 array）。</summary>
    [JsonPropertyName("mediaIdList")]
    public List<string>? MediaIdList { get; set; }

    /// <summary>手机号（<c>phoneNumber</c>，官方类型 number；敏感信息，禁止入日志）。</summary>
    [JsonPropertyName("phoneNumber")]
    public long? PhoneNumber { get; set; }

    /// <summary>
    /// 商责节点的处理形态（<c>blameResult</c>；<c>1</c> 待用户退货中 / <c>0</c> 待上传处理凭证，
    /// 仅 <see cref="ItemType"/> 为 <c>31</c>、<c>32</c> 时有意义）。
    /// </summary>
    [JsonPropertyName("blameResult")]
    public long? BlameResult { get; set; }

    /// <summary>操作者昵称（<c>nickName</c>）。</summary>
    [JsonPropertyName("nickName")]
    public string? NickName { get; set; }

    /// <summary>
    /// 处于申诉阶段节点的申诉状态（<c>appealItemType</c>；官方枚举与
    /// <see cref="WxaComplaintOrder.AppealState"/> 同一组：<c>0</c> / <c>401</c> / <c>402</c> /
    /// <c>403</c> / <c>117</c> / <c>118</c>）。
    /// </summary>
    [JsonPropertyName("appealItemType")]
    public long? AppealItemType { get; set; }
}
