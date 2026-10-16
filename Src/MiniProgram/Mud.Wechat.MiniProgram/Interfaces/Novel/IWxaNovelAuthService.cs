// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.Novel;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 小程序线「小说作品管理」域——小说授权面接口（3 端点）。
/// </summary>
/// <remarks>
/// <para>官方文档索引：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/industry/novel-api.html"/>（本域 3 支「账号-小说」授权端点，2026-10-16 逐页核验参数表与请求/应答示例）。</para>
/// <para>权限集 169；不支持云调用；支持第三方平台代商家调用（令牌恒为小程序 <c>access_token</c>，Query 注入）。</para>
/// <para><b>关系模型</b>：主授权者账号（AppID）→ 作品 ID → 被授权者账号（AppID）三维度；与 <see cref="IWxaNovelAccountAuthService"/> 的「账号-账号」两维度面<b>不同形</b>，请求体与定位键不得互换。</para>
/// <para><b>形态口径</b>：3 支官方一律 <b>POST + JSON 请求体</b>、除 <c>access_token</c> 外零业务 Query 参数；官方<b>未给出端点级频率数字</b>，只给单批/分页规模上限。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "Novel", TokenManage = nameof(IMpAppManager))]
public interface IWxaNovelAuthService
{
    /// <summary>增加账号-小说的授权关系数据，一次最多新增 20 条。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelAddBookAuthRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>逐条结果，见 <see cref="WxaNovelAddBookAuthResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_addbookauth.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/addbookauth</c>；请求体是 <c>{"books":[{book_id, grantee_appid, expire_time}]}</c> 数组包装（必填，单批上限 20 条），<b>不是</b>单条扁平对象。</para>
    /// <para><b>部分成功语义</b>：应答 <c>results[]</c> 内层各带 <c>errcode</c> ⇒ 外层 <c>errcode == 0</c> 不代表 20 条全部成立，须逐条核对（与 <see cref="IWxaNovelAccountAuthService.AddBookAuthByAppIdAsync"/> 同一形态）。</para>
    /// </remarks>
    [Post("/wxa/book/addbookauth")]
    Task<WxaNovelAddBookAuthResponse> AddBookAuthAsync([Body] WxaNovelAddBookAuthRequest request, CancellationToken cancellationToken = default);

    /// <summary>查看账号的小说授权关系列表（主授权账号使用）或被授权关系列表。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelQueryBookAuthRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>授权关系列表，见 <see cref="WxaNovelQueryBookAuthResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_querybookauth.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/querybookauth</c>；<c>type</c> 0/不填查「我授权出去的」列表、1 查「别人授权给我的」列表；官方把 <c>offset</c> / <c>count</c> 标为<b>必填</b>，单页上限 30 条。</para>
    /// <para><c>is_sum == true</c> 才返回 <c>sum</c>（查被授权列表时无需）⇒ 总数是可选回填，不得当恒定存在计数。</para>
    /// </remarks>
    [Post("/wxa/book/querybookauth")]
    Task<WxaNovelQueryBookAuthResponse> QueryBookAuthAsync([Body] WxaNovelQueryBookAuthRequest request, CancellationToken cancellationToken = default);

    /// <summary>删除账号的小说授权关系（主授权账号使用）。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelDelBookAuthRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_delbookauth.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/delbookauth</c>；应答只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</para>
    /// <para>定位键为 <c>book_id</c> + <c>grantee_appid</c> <b>两者皆必填</b>的单条删除（无批量、无「按作品全清」形态）；账号维度的删除走 <see cref="IWxaNovelAccountAuthService.DelBookAuthByAppIdAsync"/>。</para>
    /// </remarks>
    [Post("/wxa/book/delbookauth")]
    Task<WxaResponse> DelBookAuthAsync([Body] WxaNovelDelBookAuthRequest request, CancellationToken cancellationToken = default);
}
