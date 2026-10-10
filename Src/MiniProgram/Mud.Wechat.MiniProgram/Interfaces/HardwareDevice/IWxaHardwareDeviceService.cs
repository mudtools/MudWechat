// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「硬件设备」域 SDK（9 端点：设备消息 1 + 设备票据 1 + 设备组 4 + license 3）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 硬件设备，2026-10-10 依据官方清单核验）：
/// <c>hardware-device/api_*.html</c> 系列（发送设备消息 / 获取设备票据 / 设备组 / license 资源包）。
/// </para>
/// <para>
/// <b>能力口径</b>：设备消息走「设备订阅」模板（接收者为 <c>tousers</c> 数组，区别于订阅消息的
/// 单 <c>touser</c>）；设备票据 5 分钟内有效；设备组便于用户一次性订阅多个设备；license 资源包
/// 支持批量激活设备（消耗激活码序号）与批量查询剩余有效期。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：全部走应用级 <c>access_token</c>（Query）。所有端点官方均为 <b>POST</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "HardwareDevice", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaHardwareDeviceService
{
    /// <summary>
    /// 发送设备消息。官方文档：<c>hardware-device/api_sendhardwaredevicemessage.html</c>。
    /// </summary>
    /// <param name="request">接收者列表与模板内容（<c>tousers</c> 必填），见 <see cref="DataModels.HardwareDevice.WxaDeviceMessageSendRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示发送成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/cgi-bin/message/device/subscribe/send</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>前置约束（官方原文）</b>：接收者须已订阅该设备消息模板；<c>data</c> 键与模板关键词不一致报 <c>47003</c>。</para>
    /// </remarks>
    [Post("/cgi-bin/message/device/subscribe/send")]
    Task<WxaResponse> SendDeviceMessageAsync(
        [Body] DataModels.HardwareDevice.WxaDeviceMessageSendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备票据。官方文档：<c>hardware-device/api_getsnticket.html</c>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设备票据（<c>sn_ticket</c>，<b>5 分钟内有效</b>），见 <see cref="DataModels.HardwareDevice.WxaSnTicketResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/getsnticket</c>（请求体可为空对象）；Query 携带 <c>access_token</c>。</para>
    /// <para><b>票据用途（官方原文）</b>：用于生成设备码；过期后需重新获取。</para>
    /// </remarks>
    [Post("/wxa/getsnticket")]
    Task<DataModels.HardwareDevice.WxaSnTicketResponse> GetSnTicketAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建设备组。官方文档：<c>hardware-device/api_createiotgroupid.html</c>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设备组 ID 与名称，见 <see cref="DataModels.HardwareDevice.WxaIoTGroupCreateResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/business/group/createid</c>（请求体可为空对象）；Query 携带 <c>access_token</c>。</para>
    /// <para><b>能力口径（官方原文）</b>：设备组用于<b>便于用户一次性订阅多个设备</b>。</para>
    /// </remarks>
    [Post("/wxa/business/group/createid")]
    Task<DataModels.HardwareDevice.WxaIoTGroupCreateResponse> CreateIoTGroupIdAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询设备组信息。官方文档：<c>hardware-device/api_getiotgroupinfo.html</c>。
    /// </summary>
    /// <param name="request">设备组 ID（<c>group_id</c>），见 <see cref="DataModels.HardwareDevice.WxaIoTGroupInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设备组信息与组内设备列表，见 <see cref="DataModels.HardwareDevice.WxaIoTGroupInfoResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxa/business/group/getinfo</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxa/business/group/getinfo")]
    Task<DataModels.HardwareDevice.WxaIoTGroupInfoResponse> GetIoTGroupInfoAsync(
        [Body] DataModels.HardwareDevice.WxaIoTGroupInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设备组添加设备。官方文档：<c>hardware-device/api_addiotgroupdevice.html</c>。
    /// </summary>
    /// <param name="request">设备组 ID 与待添加设备列表，见 <see cref="DataModels.HardwareDevice.WxaIoTGroupDevicesRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxa/business/group/adddevice</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxa/business/group/adddevice")]
    Task<WxaResponse> AddIoTGroupDeviceAsync(
        [Body] DataModels.HardwareDevice.WxaIoTGroupDevicesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设备组删除设备。官方文档：<c>hardware-device/api_removeiotgroupdevice.html</c>。
    /// </summary>
    /// <param name="request">设备组 ID 与待删除设备列表，见 <see cref="DataModels.HardwareDevice.WxaIoTGroupDevicesRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxa/business/group/removedevice</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxa/business/group/removedevice")]
    Task<WxaResponse> RemoveIoTGroupDeviceAsync(
        [Body] DataModels.HardwareDevice.WxaIoTGroupDevicesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询 license 资源包列表。官方文档：<c>hardware-device/api_getlicensepkglist.html</c>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>已购买的 license 资源包列表，见 <see cref="DataModels.HardwareDevice.WxaLicensePkgListResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxa/business/license/getpkglist</c>（请求体可为空对象）；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxa/business/license/getpkglist")]
    Task<DataModels.HardwareDevice.WxaLicensePkgListResponse> GetLicensePkgListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 激活设备 license。官方文档：<c>hardware-device/api_activelicensedevice.html</c>。
    /// </summary>
    /// <param name="request">资源包 ID 与待激活设备序列号列表，见 <see cref="DataModels.HardwareDevice.WxaLicenseActivateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/business/license/activedevice</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>能力口径（官方原文）</b>：批量绑定设备，并消耗相应资源包中的激活码序号 —— 设备数与剩余激活数不匹配会失败。</para>
    /// </remarks>
    [Post("/wxa/business/license/activedevice")]
    Task<WxaResponse> ActiveLicenseDeviceAsync(
        [Body] DataModels.HardwareDevice.WxaLicenseActivateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询设备激活详情。官方文档：<c>hardware-device/api_getlicensedeviceinfo.html</c>。
    /// </summary>
    /// <param name="request">设备序列号列表（<c>sn_list</c>），见 <see cref="DataModels.HardwareDevice.WxaLicenseDeviceInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>批量设备剩余有效期，见 <see cref="DataModels.HardwareDevice.WxaLicenseDeviceInfoResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxa/business/license/getdeviceinfo</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxa/business/license/getdeviceinfo")]
    Task<DataModels.HardwareDevice.WxaLicenseDeviceInfoResponse> GetLicenseDeviceInfoAsync(
        [Body] DataModels.HardwareDevice.WxaLicenseDeviceInfoRequest request,
        CancellationToken cancellationToken = default);
}