// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.BusinessCapability;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 行业能力·WMPF 小程序硬件框架（2 端点：注册设备 ID + 向设备推送消息；
/// 路由族 <c>/wxa/business/runtime/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌归属与常规小程序接口不同</b>：<c>adddevice</c> 必须使用「移动应用（LauncherAPP）」
/// 身份获取的 <c>access_token</c>；<c>pushappmsg</c> 可用移动应用或小程序令牌（官方原文）。
/// 本接口仍按小程序令牌管线注入，宿主须以对应应用配置供给令牌。
/// </para>
/// <para><b>硬件生命周期</b>：设备 ID 登记成功后，激活由 WMPF 客户端 <c>activateDevice</c> 推进；
/// 推送令牌 <c>push_token</c> 由客户端 <c>getPushToken</c> 取得，属设备凭据。</para>
/// <para>
/// <b>敏感面</b>：<c>push_token</c>、设备 ID 与推送消息体一律禁入日志、遥测与异常消息。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "BusinessCapability", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaBusinessRuntimeService
{
    /// <summary>
    /// 注册 WMPF 设备 ID（<c>POST /wxa/business/runtime/adddevice</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Miniprogram_Frame/api/backend/addDevice.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaBusinessRuntimeAddDeviceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>统一错误信封，见 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>令牌必须为移动应用身份</b>：用小程序令牌调用将返回权限错误；本 SDK 不校验令牌归属，
    /// 由宿主应用配置保证。
    /// </para>
    /// <para><b>错误码</b>：<c>50093</c> 数据异常 / <c>50097</c> 无权限 / <c>50098</c> 设备超限 / <c>9402101</c> 产品不存在。</para>
    /// </remarks>
    [Post("/wxa/business/runtime/adddevice")]
    Task<WxaResponse> AddDeviceAsync(
        [Body] WxaBusinessRuntimeAddDeviceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 向 WMPF 推送消息（<c>POST /wxa/business/runtime/pushappmsg</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Miniprogram_Frame/api/backend/pushAppMsg.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaBusinessRuntimePushAppMsgRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>统一错误信封，见 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>消息体上限 5KB</b>（<c>45002</c> 表示超限）；<c>48012</c> 表示推送目标无效。</para>
    /// <para><b>非幂等</b>：重复推送会重复触达设备。</para>
    /// </remarks>
    [Post("/wxa/business/runtime/pushappmsg")]
    Task<WxaResponse> PushAppMsgAsync(
        [Body] WxaBusinessRuntimePushAppMsgRequest request,
        CancellationToken cancellationToken = default);
}
