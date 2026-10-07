// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 自定义菜单事件载荷（<c>MsgType = event</c>，官方自定义菜单事件推送页 9 键合并登记）。
/// </summary>
/// <remarks>
/// <para>
/// <b>标量字段</b>：<c>EventKey</c>（菜单 KEY / 跳转 URL / 小程序路径）与 <c>MenuId</c>（跳转类事件携带）。
/// </para>
/// <para>
/// <b>嵌套结构（<c>ScanCodeInfo</c> / <c>SendPicsInfo</c> / <c>SendLocationInfo</c>）本轮经
/// <see cref="MpCallbackPayload.Values"/> 全量值袋读取</b>：这些节点为「容器 + 重复项」的包装形态，
/// 其类型化建模须与叶层投影器的「包装形态豁免表」同批登记（与企微侧
/// <c>WechatCallbackScanCodeInfo</c> 等子节点 DTO 同款）；在官方字段表逐页核验完成前，
/// 以值袋暴露可避免用未核验的字段名建模（方案 §12 纪律：不得凭记忆补齐字段）。
/// </para>
/// <para>
/// <b>不登记</b> <c>media_id</c>/<c>view_limited</c>：本轮官方菜单事件页未列出，须逐页核验后增量补入（方案 §12 V6）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[]
{
    MpCallbackEventTypes.Click,
    MpCallbackEventTypes.View,
    MpCallbackEventTypes.ScanCodePush,
    MpCallbackEventTypes.ScanCodeWaitMsg,
    MpCallbackEventTypes.PicSysPhoto,
    MpCallbackEventTypes.PicPhotoOrAlbum,
    MpCallbackEventTypes.PicWeixin,
    MpCallbackEventTypes.LocationSelect,
    MpCallbackEventTypes.ViewMiniProgram,
})]
public sealed partial class MpMenuEventPayload : MpCallbackPayload
{
    /// <summary>
    /// 事件 KEY 值：<c>CLICK</c> 为菜单 KEY；<c>VIEW</c> 为跳转 URL；<c>view_miniprogram</c> 为小程序路径；
    /// <c>scancode_*</c> / <c>pic_*</c> / <c>location_select</c> 为菜单配置的 KEY。
    /// </summary>
    [PayloadField("EventKey")]
    public string? EventKey { get; set; }

    /// <summary>菜单 ID（跳转类事件 <c>VIEW</c> / <c>view_miniprogram</c> 携带）。</summary>
    [PayloadField("MenuId")]
    public string? MenuId { get; set; }
}
