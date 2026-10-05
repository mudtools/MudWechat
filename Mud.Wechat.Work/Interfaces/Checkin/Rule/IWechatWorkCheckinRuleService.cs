// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Checkin;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「打卡」模块打卡规则域公共 SDK（获取员工打卡规则）。
/// <para>
/// 官方对三类应用开放一致的「获取员工打卡规则」端点，收敛声明于本接口；应用类型子接口承载官方开放面差异端点：
/// 企业自建应用见 <see cref="IWechatWorkInternalCheckinRuleService"/>（额外开放获取企业所有打卡规则与管理打卡规则 4 端点），
/// 服务商代开发见 <see cref="IWechatWorkProviderCheckinRuleService"/>（额外开放获取企业所有打卡规则与管理打卡规则 4 端点），
/// 第三方应用见 <see cref="IWechatWorkThirdPartyCheckinRuleService"/>（官方权限表对获取企业所有打卡规则与管理打卡规则均标注暂不支持，零差异端点空标记）。
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
public interface IWechatWorkCheckinRuleService
{
    /// <summary>
    /// 获取员工打卡规则
    /// <para>获取可见范围内指定员工指定日期的打卡规则（用户在不同日期的规则不一定相同，请按天获取）。</para>
    /// <para>官方限制：用户列表不超过 100 个，若用户超过 100 个，请分批获取。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetCheckinOptionRequest"/>：datetime 需要获取规则的日期当天 0 点 Unix 时间戳 / useridlist 用户列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>打卡规则列表（info：userid 打卡人员 userid / group 打卡规则相关信息）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90263"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94204"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99443"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用与第三方应用须具有「打卡」权限。</para>
    /// <para>官方契约陷阱：返回示例出现字面量点号键 "group.checkin_method_type"（官方示例原文如此），实际应为嵌套字段；参数表类型标注存在 work_se、uint3、unit32 等官方笔误（照抄勿修正）。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/getcheckinoption")]
    Task<GetCheckinOptionResponse> GetCheckinOptionAsync(
        [Body] GetCheckinOptionRequest request,
        CancellationToken cancellationToken = default);
}
