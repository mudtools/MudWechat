// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「打卡」模块打卡记录域公共 SDK（公共面收敛接口，零端点）。
/// <para>
/// 官方对三种应用类型开放的同路由端点「获取打卡记录数据」（<c>/cgi-bin/checkin/getcheckindata</c>）
/// 返回结构<b>不同构</b>：自建应用与服务商代开发文档页（90262/96497）为现代字段结构，
/// 第三方应用文档页（94205）为旧字段结构（agency_name / location_title_lat / location_title_lng，
/// 无 lat/lng/deviceid/sch_checkin_time）；同路由不同响应形态无法收敛进父接口，
/// 故本接口零端点，全部端点由应用类型子接口承载：
/// 企业自建应用见 <see cref="IWechatWorkInternalCheckinRecordService"/>（另开放补卡/添加打卡记录/录入人脸 3 个仅自建端点），
/// 服务商代开发见 <see cref="IWechatWorkProviderCheckinRecordService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyCheckinRecordService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：自建应用须配置到
/// 「打卡 - 可调用接口的应用」中；代开发应用与第三方应用须具有「打卡」权限。
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkCheckinRecordService
{
}
