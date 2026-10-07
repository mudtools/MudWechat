// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「用户管理 → 标签管理」域 SDK（8 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/subscription/api/"/>
/// → 用户管理 → 标签管理。各端点 remarks 中的深链为**逐页核验过的真实页面**
/// （官方页 slug 不可由接口名推导，故未核验者不写假链接，改以「分组 + 官方接口英文名 + 请求路径」定位）。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>账号资格</b>：本域全部端点适用范围均为「公众号 / 服务号 —— <b>仅认证</b>」，
/// 即仅允许<b>企业主体已认证</b>账号调用；未认证或不支持认证的账号无法调用（官方原注）。</item>
/// <item><b>标签数量</b>：一个公众号<b>最多创建 100 个标签</b>（超限错误码 <c>45056</c>）。</item>
/// <item><b>标签名长度</b>：<c>tag.name</c> 官方限制 <b>30 个字符以内</b>（超长错误码 <c>45158</c>）。</item>
/// <item><b>单用户标签数</b>：官方「注意事项」原文为「标签功能目前支持公众号为用户打上
/// <b>最多 20 个标签</b>」（超限错误码 <c>45059</c>）。</item>
/// <item><b>系统标签</b>：系统标签禁止修改 / 删除（错误码 <c>45058</c>）⇒ 调用方应先经
/// <see cref="GetTagsAsync"/> 读取现有标签，不要对非自建标签做增删改。</item>
/// <item><b>频率限制</b>：官方本域各页<b>均未声明</b>独立 QPS / 频次上限，仅在错误码中体现
/// <c>45009</c>（每日调用上限）。故本 SDK 不做频次承诺（<b>不编造数值</b>）。</item>
/// <item><b>第三方调用</b>：本域支持第三方平台代商家调用（<c>authorizer_access_token</c>，
/// 权限集 id = 2、100）；本 SDK 当前仅覆盖自建令牌形态。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：8 个端点统一消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；与基础域同源的 MUD005 已知接受风险）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Tag", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpTagService
{
    /// <summary>
    /// 创建标签。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_createtag.html"/>
    /// （官方接口英文名 <c>createTag</c>）。
    /// </summary>
    /// <param name="request">标签信息（<c>tag.name</c> 必填，<b>30 个字符以内</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建标签（仅 <c>tag.id</c> 与 <c>tag.name</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方请求体示例 <c>{ "tag": { "name": "广东" } }</c>，返回示例 <c>{ "tag": { "id": 134, "name": "广东" } }</c>。
    /// </para>
    /// <para>
    /// 官方业务约束：一个公众号<b>最多创建 100 个标签</b>（错误码 <c>45056 标签数超限</c>）；
    /// 标签名 30 字符以内（错误码 <c>45157</c> / <c>45158</c>）；响应<b>不含</b> <c>count</c> 字段。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>45009</c> / <c>45056</c> / <c>45157</c> / <c>45158</c> / <c>48001</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/tags/create")]
    Task<MpCreateTagResponse> CreateTagAsync(
        [Body] MpCreateTagRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取标签。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_gettags.html"/>
    /// （官方接口英文名 <c>getTags</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标签列表（<c>tags[].id</c> / <c>name</c> / <c>count</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET 且无请求体</b>（故不设请求 DTO——勿为「统一风格」加请求体）；
    /// 响应 <c>tags</c> 元素字段为 <c>id</c>(number) / <c>name</c>(string) / <c>count</c>(number，标签下粉丝数)。
    /// </para>
    /// <para>官方「注意事项」章节原文为「本接口无特殊注意事项」。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/tags/get")]
    Task<MpGetTagsResponse> GetTagsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑标签。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_updatetag.html"/>
    /// （官方接口英文名 <c>updateTag</c>）。
    /// </summary>
    /// <param name="request">标签信息（<c>tag.id</c> 与 <c>tag.name</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方业务约束：系统标签禁止修改（<c>45058 can't modify sys tag</c>）；标签名校验
    /// （<c>45157 invalid tag name</c> / <c>45158 tag name too long</c>）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>41001</c> / <c>42001</c> / <c>44002</c> / <c>45009</c> /
    /// <c>45058</c> / <c>45157</c> / <c>45158</c> / <c>47001</c> / <c>48001</c>。
    /// </para>
    /// <para>官方「注意事项」章节原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/tags/update")]
    Task<MpResponse> UpdateTagAsync(
        [Body] MpUpdateTagRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除标签。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_deletetag.html"/>
    /// （官方接口英文名 <c>deleteTag</c>）。
    /// </summary>
    /// <param name="request">待删除标签（<c>tag.id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方业务约束：系统标签禁止删除（<c>45058</c>）；删除后该标签下的粉丝关系随之解除
    /// （官方未提供批量解除标签的独立端点）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>40001</c> / <c>41001</c> / <c>42001</c> / <c>44002</c> / <c>45009</c> / <c>45058</c> /
    /// <c>47001</c> / <c>48001</c> / <c>61004</c>。
    /// </para>
    /// <para>
    /// <b>官方文档缺陷提示</b>：该页字段表把 <c>tag</c> / <c>tag.id</c> 的「必填」列标为「否」，
    /// 但语义上必须提供（否则 <c>44002 empty post data</c>）⇒ SDK 按必填建模，不跟随错误标注。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/tags/delete")]
    Task<MpResponse> DeleteTagAsync(
        [Body] MpDeleteTagRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取标签下粉丝列表。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_gettagfans.html"/>
    /// （官方接口英文名 <c>getTagFans</c>）。
    /// </summary>
    /// <param name="request">分页请求（<c>tagid</c> 必填；<c>next_openid</c> 不填默认从头开始拉取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>本页粉丝（<c>count</c> / <c>data.openid</c> / <c>next_openid</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 分页语义：响应 <c>next_openid</c> 为「拉取列表最后一个用户的 openid」⇒ 下一页须把它作为
    /// 请求 <c>next_openid</c> 回传；官方未声明单页上限，故以 <c>count</c> 与 <c>next_openid</c>
    /// 组合判定是否继续翻页（<c>next_openid</c> 为空即到底）。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>40003</c> / <c>45159</c>（非法的 tag_id）。</para>
    /// <para>
    /// <b>官方文档缺陷提示</b>：字段表把 <c>tagid</c> / <c>next_openid</c> 的「必填」列均标为「否」，
    /// 但 <c>tagid</c> 语义必填（否则 <c>45159</c>）⇒ SDK 按必填建模。
    /// </para>
    /// <para>官方「注意事项」章节原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/user/tag/get")]
    Task<MpGetTagFansResponse> GetTagFansAsync(
        [Body] MpGetTagFansRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量为用户打标签。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_batchtagging.html"/>
    /// （官方接口英文名 <c>batchTagging</c>）。
    /// </summary>
    /// <param name="request">请求体（<c>openid_list</c> <b>最多 50 个</b>；<c>tagid</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>，部分失败时含 <c>fail_openid_list</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方业务约束（逐项核验）：<c>openid_list</c> <b>最多 50 个</b>；
    /// 单用户标签数<b>最多 20 个</b>（<c>45059 can not tagging one user too much</c>）；
    /// 同一 openid 并发打标 / 取消标签会触发 <c>45169 openid much req</c>（调用方应串行化同一 openid 的变更）。
    /// </para>
    /// <para>
    /// <b>部分失败</b>：<c>45171 some openid fail</c> 表示部分 openid 失败，响应体 <c>fail_openid_list</c>
    /// 给出失败清单 ⇒ 应<b>定向重试</b>而非整批重放（整批重放会重复打标已成功的用户）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40003</c> / <c>40032</c> / <c>41001</c> / <c>42001</c> /
    /// <c>44002</c> / <c>45009</c> / <c>45011</c> / <c>45059</c> / <c>45159</c> / <c>45169</c> / <c>45171</c> /
    /// <c>47001</c> / <c>48001</c> / <c>49003</c> / <c>50002</c> / <c>50005</c> / <c>61007</c> / <c>61016</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/tags/members/batchtagging")]
    Task<MpTagMembersResponse> BatchTaggingAsync(
        [Body] MpTagMembersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量为用户取消标签。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_batchuntagging.html"/>
    /// （官方接口英文名 <c>batchUntagging</c>）。
    /// </summary>
    /// <param name="request">请求体（<c>openid_list</c>；<c>tagid</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>，部分失败时含 <c>fail_openid_list</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>与打标签侧的差异（勿「顺手对齐」）</b>：官方本页字段表<b>未给出</b> <c>openid_list</c> 数量上限，
    /// 仅在错误码中列 <c>40032</c>（不合法的 openid 列表长度）⇒ 本 SDK 不把「最多 50 个」写成官方对
    /// 取消标签的明文约束（那是 <c>batchTagging</c> 页的声明）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40003</c> / <c>40032</c> / <c>41001</c> / <c>42001</c> /
    /// <c>45009</c> / <c>45011</c> / <c>45159</c> / <c>45169</c> / <c>45171</c> / <c>47001</c> / <c>48001</c> /
    /// <c>49003</c> / <c>61016</c>。
    /// </para>
    /// <para>官方「注意事项」章节原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/tags/members/batchuntagging")]
    Task<MpTagMembersResponse> BatchUnTaggingAsync(
        [Body] MpTagMembersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户的标签列表。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_getidlist.html"/>
    /// （官方接口英文名 <c>getTagidList</c>）。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户的标签 id 列表（<c>tagid_list</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 本地化提示：若需一次取多个用户的标签，官方<b>无</b>批量端点 ⇒ 只能逐用户调用
    /// （与 <c>user/info/batchget</c> 不同，本域无批量形态），调用方应自行控频
    /// （官方本域无独立频次声明，仅 <c>45009</c> 每日总量）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/tags/getidlist")]
    Task<MpGetTagIdListResponse> GetTagIdListAsync(
        [Body] MpGetTagIdListRequest request,
        CancellationToken cancellationToken = default);
}
