// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Checkin;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「打卡」模块打卡规则域企业自建应用 SDK。
/// <para>
/// 官方对本应用类型在三类公共面（<see cref="IWechatWorkCheckinRuleService"/>）之外额外开放 5 个差异端点：
/// 「获取企业所有打卡规则」与「管理打卡规则」4 个写端点（创建/修改/清空数组元素/删除打卡规则，
/// 官方权限表对第三方应用标注暂不支持，代开发应用亦开放，见 <see cref="IWechatWorkProviderCheckinRuleService"/>）。
/// </para>
/// <para>第三方应用见 <see cref="IWechatWorkThirdPartyCheckinRuleService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用须配置到「打卡 - 可调用接口的应用」中。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Checkin",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCheckinRuleService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalCheckinRuleService : IWechatWorkCheckinRuleService
{
    /// <summary>
    /// 获取企业所有打卡规则
    /// <para>获取企业内所有打卡规则（含打卡范围、加班信息、汇报对象等管理字段）。</para>
    /// <para>官方限制：接口调用频率限制为 60 次/分钟。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetCorpCheckinOptionRequest"/>：官方请求体为空 JSON 对象，仅 Query 上携带 access_token）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>企业规则信息列表（group：规则类型 / 打卡时间配置 / 特殊日期 / 打卡地点 / 打卡范围 range / 旧版加班信息 ot_info / 汇报对象 reporterinfo 等）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93384"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99444"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用须具有「打卡」权限；官方权限表对第三方应用标注暂不支持。</para>
    /// <para>官方契约陷阱：checkindate.checkintime 参数表类型标注 uint32 实为对象数组；range.userid 参数表标注 string 实为字符串数组；响应侧加班信息为旧 ot_info 结构（官方错误表明确 ot_info 是旧字段不建议使用，与请求侧 ot_info_v2 不同构）。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/getcorpcheckinoption")]
    Task<GetCorpCheckinOptionResponse> GetCorpCheckinOptionAsync(
        [Body] GetCorpCheckinOptionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建打卡规则
    /// <para>为企业添加打卡规则；创建新规则后，打卡应用内将生成对应规则。</para>
    /// <para>官方限制：创建打卡规则时 groupid 无需传入，该字段会被忽略；wifimac_infos 与 loc_infos 不能同时为空；wifi/地点个数不超过 500；checkindate 个数 1~7；单日打卡时段 1~4；排班个数不超过 50；特殊工作日/非工作日个数不超过 366；主要错误码 301093（参数校验失败）、301094（使用旧字段/旧特性）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddCheckinOptionRequest"/>：group 打卡规则详细定义 / effective_now 是否立即生效）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98041"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98767"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用须具有「打卡」权限；官方权限表对第三方应用标注暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/add_checkin_option")]
    Task<WechatWorkResponse> AddCheckinOptionAsync(
        [Body] AddCheckinOptionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改打卡规则
    /// <para>修改该应用为企业创建的打卡规则。</para>
    /// <para><b>危险操作约束</b>：打卡规则仅可由该规则的创建应用修改。官方更新语义——修改时 group 须传入 groupid 否则报错；
    /// 对 group.* 一级的字段：数组字段「不传/传空 = 不更新、传值 = 覆盖（清空原有数组，保留传入的数组）」；
    /// 非数组字段「传值 = 覆盖（及递归的所有字段）、不传 = 不更新」；
    /// 若想清空 group.* 一级的数组字段须使用清空打卡规则数组元素接口，非数组字段直接传入空元素即可。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateCheckinOptionRequest"/>：group 打卡规则详细定义（须含 groupid） / effective_now 是否立即生效）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98041"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98767"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用须具有「打卡」权限；官方权限表对第三方应用标注暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/update_checkin_option")]
    Task<WechatWorkResponse> UpdateCheckinOptionAsync(
        [Body] UpdateCheckinOptionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空打卡规则数组元素
    /// <para>清空打卡规则的一级数组字段（修改打卡规则接口无法将数组字段清空为空集，须使用本接口）。</para>
    /// <para>官方限制：打卡规则仅可由该规则的创建应用修改；清空字段标识：1 - 清空 spe_workdays；2 - 清空 spe_offdays；3 - 清空 wifimac_infos；4 - 清空 loc_infos（wifimac_infos 和 loc_infos 不可同时为空）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ClearCheckinOptionArrayFieldRequest"/>：groupid 打卡规则 id / clear_field 清空的字段标识列表 / effective_now 是否立即生效）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98041"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98767"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用须具有「打卡」权限；官方权限表对第三方应用标注暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/clear_checkin_option_array_field")]
    Task<WechatWorkResponse> ClearCheckinOptionArrayFieldAsync(
        [Body] ClearCheckinOptionArrayFieldRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除打卡规则
    /// <para>删除该应用为企业创建的打卡规则。</para>
    /// <para>官方限制：打卡规则仅可由该规则的创建应用删除。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DelCheckinOptionRequest"/>：groupid 删除的打卡规则 id / effective_now 是否立即生效）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98041"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98767"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用须具有「打卡」权限；官方权限表对第三方应用标注暂不支持。</para>
    /// <para>官方契约陷阱：删除打卡规则端点的官方导语与修改端点相同（「修改该应用为企业创建的打卡规则」），官方原文如此。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/del_checkin_option")]
    Task<WechatWorkResponse> DelCheckinOptionAsync(
        [Body] DelCheckinOptionRequest request,
        CancellationToken cancellationToken = default);
}
