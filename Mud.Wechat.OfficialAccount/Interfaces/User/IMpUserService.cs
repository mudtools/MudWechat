// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「用户管理 → 用户信息」域 SDK（7 端点，含黑名单三端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/subscription/api/"/>
/// → 用户管理 → 用户信息。各端点 remarks 中的深链均为**逐页核验过的真实页面**。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>账号资格</b>：本域全部端点适用范围为「公众号 / 服务号 —— <b>仅认证</b>」；
/// 但 <c>updateRemark</c> 页正文另有一句更窄的原文——「该接口<b>暂时开放给微信认证的服务号</b>」，
/// 即该端点实际开放面可能窄于其余 6 个（调用方按服务号认证形态使用最稳）。</item>
/// <item><b>昵称 / 头像已停供</b>：官方明示「<b>2021 年 12 月 27 日之后，不再输出头像、昵称信息</b>」
/// ⇒ SDK <b>不建模</b> <c>nickname</c>/<c>sex</c>/<c>city</c>/<c>province</c>/<c>country</c>/<c>headimgurl</c>
/// （建模它们只会暗示「也许能读到」）。</item>
/// <item><b>批量上限</b>：<c>user/info/batchget</c> 单次最多 <b>100</b> 条；
/// <c>user/get</c>（获取关注者列表）单次最多 <b>10000</b> 条；<c>tags/members/getblacklist</c>
/// 单次最多 <b>1000</b> 条；拉黑 / 取消拉黑单次最多 <b>20</b> 个。</item>
/// <item><b>分页终止</b>：<c>user/get</c> 官方原文「最后一次返回时 <c>next_openid</c> 可能为空表示列表结束」；
/// 黑名单端点请求游标名为 <c>begin_openid</c>、响应游标为 <c>next_openid</c>（<b>名字不同，勿互相「对齐」</b>）。</item>
/// <item><b>频率限制</b>：官方本域各页均<b>未声明</b>独立 QPS / 频次上限，仅错误码 <c>45009</c>（每日调用上限）
/// ⇒ SDK 不编造频次数值；<c>user/get</c> 的分页应串行推进并自行控频。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "User", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpUserService
{
    /// <summary>
    /// 获取用户基本信息。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_userinfo.html"/>
    /// （官方接口英文名 <c>userInfo</c>）。
    /// </summary>
    /// <param name="openid">普通用户的标识，对当前公众号唯一。</param>
    /// <param name="lang">返回国家地区语言版本（<c>zh_CN</c> / <c>zh_TW</c> / <c>en</c>）；留空由官方取默认。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户信息（字段平铺，出错时附 <c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，Query <c>openid</c>（必填）/ <c>lang</c>（可选）；<b>无请求体</b>。
    /// </para>
    /// <para>
    /// <b><c>subscribe = 0</c></b> 表示该用户未关注，官方原文「拉取不到其余信息」⇒ 调用方须先判该字段。
    /// </para>
    /// <para>
    /// <b>昵称 / 头像 / 性别 / 地区字段已停供</b>（2021-12-27 起）；<c>language</c> 字段页面标注
    /// 「该字段不再提供」（SDK 保留为可空以对齐官方字段表）。
    /// </para>
    /// <para><c>unionid</c> 仅当公众号绑定微信开放平台账号后才出现。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>40003</c>（invalid openid）/ <c>40013</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/user/info")]
    Task<MpUserInfoResponse> GetUserInfoAsync(
        [Query("openid")] string openid,
        [Query("lang")] string? lang = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取用户基本信息。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_batchuserinfo.html"/>
    /// （官方接口英文名 <c>batchUserinfo</c>）。
    /// </summary>
    /// <param name="request">用户列表（<b>单次最多 100 条</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户信息列表（<c>user_info_list</c>，元素字段与单查端点同集）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，请求体 <c>{"user_list":[{"openid":…,"lang":…}]}</c>，
    /// 响应 <c>{"user_info_list":[…]}</c>；元素字段与「获取用户基本信息」<b>完全同集</b>
    /// （故 SDK 共用 <c>MpUserInfo</c> 元素类型，避免两份会漂移的声明）。
    /// </para>
    /// <para>
    /// 官方提示：返回列表内可能<b>同时含已关注与未关注用户</b>（后者 <c>subscribe = 0</c>）⇒
    /// 调用方须逐条判 <c>subscribe</c>，且<b>不得据「列表长度 == 请求长度」推断全部成功</b>。
    /// </para>
    /// <para>
    /// <b>单次上限 100 条</b>（超限错误码 <c>40032 invalid openid list size</c>）；
    /// <c>user_list[].openid</c> 必须是<b>已关注</b>用户的 openid（否则 <c>40003</c>）。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>40003</c> / <c>40013</c> / <c>40032</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/user/info/batchget")]
    Task<MpBatchGetUserInfoResponse> BatchGetUserInfoAsync(
        [Body] MpBatchGetUserInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取关注用户列表。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_getfans.html"/>
    /// （官方接口英文名 <c>getFans</c>）。
    /// </summary>
    /// <param name="nextOpenId">上一批列表的最后一个 OPENID；留空表示从头开始拉取。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>关注者分页（<c>total</c> / <c>count</c> / <c>data.openid</c> / <c>next_openid</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，Query <c>next_openid</c>（可选）；<b>无请求体</b>。
    /// </para>
    /// <para>
    /// <b>单次最多 10000 个</b>；官方「注意事项」原文第 2 条：「<b>最后一次返回时 <c>next_openid</c>
    /// 可能为空表示列表结束</b>」⇒ 翻页终止条件为响应 <c>next_openid</c> 为空。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40003</c> / <c>40013</c> / <c>41001</c> / <c>42001</c> /
    /// <c>45009</c> / <c>48001</c> / <c>50002</c> / <c>61004</c> / <c>61007</c> / <c>61016</c> / <c>268487002</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/user/get")]
    Task<MpGetFansResponse> GetFansAsync(
        [Query("next_openid")] string? nextOpenId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置用户备注名。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_updateremark.html"/>
    /// （官方接口英文名 <c>updateRemark</c>）。
    /// </summary>
    /// <param name="request">备注请求（<c>openid</c> 与 <c>remark</c> 均必填；<c>remark</c> 字节数须 &lt; 30）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>开放面窄于同域其余端点</b>：官方正文原文「本接口用于对指定用户设置备注名，该接口<b>暂时开放给
    /// 微信认证的服务号</b>」——而字段级「适用范围」表仍写「公众号 / 服务号 均仅认证」。
    /// SDK 以正文原文为准做提示，不额外做账号类型拦截（由官方返回 <c>48001</c> 表达）。
    /// </para>
    /// <para>
    /// <b>备注名长度按字节</b>：官方原文「长度必须小于 30 字节」（见
    /// <c>MpUpdateRemarkRequest.RemarkByteLimit</c>）；SDK 不自动截断。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>0</c> / <c>40001</c> / <c>40003</c> / <c>40013</c> / <c>40092</c>（备注名非法）/
    /// <c>41001</c> / <c>42001</c> / <c>43002</c>（须用 POST）/ <c>43004</c>（该 openid 未关注当前账号）/
    /// <c>44002</c> / <c>45009</c> / <c>47001</c> / <c>48001</c> / <c>61016</c>。
    /// </para>
    /// <para>官方「注意事项」章节原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/user/info/updateremark")]
    Task<MpResponse> UpdateRemarkAsync(
        [Body] MpUpdateRemarkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取公众号的黑名单列表。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_getblacklist.html"/>
    /// （官方接口英文名 <c>getBlacklist</c>）。
    /// </summary>
    /// <param name="request">分页请求（<c>begin_openid</c> 可选，为空时从开头拉取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>黑名单分页（<c>total</c> / <c>count</c> / <c>data.openid</c> / <c>next_openid</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方「注意事项」原文：①「每次最多拉取 <b>1000</b> 个 OpenID」；②「通过多次拉取满足需求」；
    /// ③「<c>begin_openid</c> 为空时默认从头开始」。
    /// </para>
    /// <para>
    /// <b>游标字段名不对称</b>：请求为 <c>begin_openid</c>、响应为 <c>next_openid</c> ⇒ 下一页须把响应的
    /// <c>next_openid</c> 回填到请求的 <c>begin_openid</c>（字段名照抄官方，勿互相「对齐」）。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>0</c> / <c>40001</c> / <c>40003</c> / <c>49003</c>（openid 不属于此 AppID）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/tags/members/getblacklist")]
    Task<MpGetBlacklistResponse> GetBlacklistAsync(
        [Body] MpGetBlacklistRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拉黑用户。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_batchblacklist.html"/>
    /// （官方接口英文名 <c>batchBlacklist</c>）。
    /// </summary>
    /// <param name="request">待拉黑 openid 列表（<b>单次最多 20 个</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方「注意事项」原文：「一次最多拉黑 <b>20</b> 个用户」（超限错误码 <c>40032</c>）。</para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40003</c> / <c>40032</c> / <c>41001</c> / <c>42001</c> /
    /// <c>44002</c> / <c>47001</c> / <c>48001</c> / <c>49003</c> / <c>50002</c> / <c>61016</c> / <c>268487001</c>。
    /// </para>
    /// <para>
    /// <c>268487001</c> / <c>268487002</c> 为官方新增的<b>平台级系统繁忙码</b>（非 4xxxx / 6xxxx 段），
    /// 解决方案均为「稍后重试」⇒ 建议纳入可重试错误集合。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/tags/members/batchblacklist")]
    Task<MpResponse> BatchBlacklistAsync(
        [Body] MpBlacklistRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消拉黑用户。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_batchunblacklist.html"/>
    /// （官方接口英文名 <c>batchUnblacklist</c>）。
    /// </summary>
    /// <param name="request">待取消拉黑 openid 列表（<b>单次最多 20 个</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方「注意事项」原文：「一次最多取消 <b>20</b> 个用户」（<c>40032</c> 的错误描述亦为「一次只能取消拉黑 20 个用户」）。</para>
    /// <para>
    /// <b>与本域其余端点的不同</b>：官方本页错误码表<b>仅列 3 项</b>（<c>40001</c> / <c>40003</c> / <c>40032</c>），
    /// 未列 <c>45009</c> 等通用码 ⇒ SDK 不据其他页「补全」错误码清单。
    /// </para>
    /// <para>官方「注意事项」章节原文即「一次最多取消 20 个用户」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/tags/members/batchunblacklist")]
    Task<MpResponse> BatchUnblacklistAsync(
        [Body] MpBlacklistRequest request,
        CancellationToken cancellationToken = default);
}
