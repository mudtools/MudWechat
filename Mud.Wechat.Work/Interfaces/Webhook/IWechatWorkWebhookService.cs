// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「群机器人」模块接口族公共 SDK（官方「服务端API → 群机器人」，消息推送配置说明见 91770）。
/// <para>
/// 群机器人（Webhook 机器人）由用户在群聊设置中添加，官方以 Webhook URL 上的 <c>key</c> 为机器人唯一凭据；
/// Webhook 推送<b>不属于任何应用</b>、与 access_token / suite_access_token / provider_access_token
/// 三条令牌链路均无关，官方文档亦不区分自建 / 第三方 / 代开发开放面
/// ⇒ 本域形态对齐 <see cref="IWechatWorkAibotService"/>：零端点父接口 + 唯一自建子接口承载端点，
/// 继承链上不得出现代开发 / 第三方子接口。
/// </para>
/// <para>
/// 端点全部声明于 <see cref="IWechatWorkInternalWebhookService"/>；
/// 父接口不进入令牌归属域守卫 TO1 的枚举面，子接口的无 <c>[Token]</c> 例外登记见 <c>WechatTokenOwnerContractGuards</c> TO1。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌面：本域端点不使用任何令牌链路</b>。官方 <c>/cgi-bin/webhook/send</c> 与
/// <c>/cgi-bin/webhook/upload_media</c> 均以 URL Query 上的 <c>key</c> 为机器人凭据，
/// 故本域父 / 子接口<b>均不声明 <c>[Token]</c></b> —— 这是「无令牌端点」的第三个既存例外
/// （前两个为 <see cref="IWechatWorkAibotService"/>：response_code 一次性凭据；
/// <see cref="IWechatWorkProviderAuthenticationUrl"/>：令牌经显式 Query 参数直传）。
/// 组件分析器的 HTTPCLIENT018（建议补 <c>[Token]</c>）对本域为不适用告警，已在声明处局部 <c>#pragma</c> 豁免。
/// </para>
/// <para>
/// <b>无令牌 ≠ 不参与多应用切换</b>：<c>TokenManage</c> 只配置应用切换管理器（生成器据此发射
/// <c>UseApp</c> / <c>BeginScope</c>），与令牌注入无关；与全仓各域父接口形态一致（恒为
/// <c>nameof(IWechatAppManager)</c>），且父子两级必须统一——否则生成器以 <c>new</c> 隐藏基类切换成员
/// （组件分析器 HTTPCLIENT028）。
/// </para>
/// <para>
/// <b>官方凭证脱敏缺口（已知接受风险，追踪号 WEBHOOK-KEY-REDACT-01）</b>：
/// <c>key</c> 是<b>长期有效</b>的 URL 凭据（与 response_code 的一次性窗口不同），但其参数名
/// 不在组件 <c>SensitiveUrlRedactor</c> 脱敏词表内（该词表为精确匹配、组件侧 NuGet 单一版本锁定，
/// 3.0.1 词表实测不含 <c>key</c>；且 <c>key</c> 一词过于通用，直接进全局词表会过度脱敏）。
/// SDK 侧不得抢占组件脱敏面，按 G7 同源决策登记为显式豁免（理由 + 追踪号见守卫 WEB3）；
/// 组件词表若后续覆盖 <c>key</c>，守卫 WEB3 会红并要求清理本豁免。
/// </para>
/// </remarks>
// HTTPCLIENT018 豁免理由：本域为「无令牌端点」既存例外（官方 91770 以 URL Query 上的 key 为机器人凭据，
// 与任何令牌链路均无关，见上方 remarks 与守卫 WEB3），分析器唯一消警路径「补 [Token]」会让生成器注入
// 本不该存在的令牌参数、违反域契约；此处仅本接口局部豁免，全仓其余接口的 018 检查不受影响。
#pragma warning disable HTTPCLIENT018
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
public interface IWechatWorkWebhookService
{
}
#pragma warning restore HTTPCLIENT018
