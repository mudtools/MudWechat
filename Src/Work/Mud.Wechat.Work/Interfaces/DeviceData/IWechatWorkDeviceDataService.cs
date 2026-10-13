// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「智能设备数据」域公共 SDK（官方「智慧硬件服务端」文档树 <c>/cgi-bin/devicedata/*</c> 路由族）。
/// <para>
/// 官方「智慧硬件服务端」分类 96107 的「接口调用」页面只在<b>第三方应用开发</b>根下存在
/// （96097 获取硬件授权结果 / 96027 获取考勤打卡原始数据 / 96028 获取温度检测原始数据 / 96029 获取门禁通行原始数据 /
/// 96030 读取门禁通行规则 / 96031 写入门禁通行规则 / 96221 修改门禁通行规则 / 96227 删除门禁通行规则），
/// 企业自建应用与服务商代开发两棵树均无对应页面 ⇒ 本父接口零端点、亦不设自建 / 代开发子接口，
/// 全部 8 端点声明于唯一的第三方子接口 <c>IWechatWorkThirdPartyDeviceDataService</c>
/// （形态对齐 <see cref="IWechatWorkKfComponentService"/> 与 <see cref="IWechatWorkDialService"/> 的零端点父接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 第三方应用消费授权企业级令牌（scope = authCorpId），由多应用基座按当前应用上下文路由。
/// </para>
/// <para>
/// 与「智能硬件开发」根树下的 <c>/cgi-bin/openhw/*</c> 分属官方两棵章节树：openhw 侧凭据链是
/// <c>model_token</c> / <c>device_token</c> 与设备签名，不属三类应用令牌面，故不在本域承载。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkDeviceDataService
{
}
