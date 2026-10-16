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
/// 小程序线「小说作品管理」域——阅读器面接口（3 端点）。
/// </summary>
/// <remarks>
/// <para>官方文档索引：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/industry/novel-api.html"/>（本域 3 支阅读器端点，2026-10-16 逐页核验参数表与请求/应答示例）。</para>
/// <para>权限集 169；不支持云调用；支持第三方平台代商家调用（令牌恒为小程序 <c>access_token</c>，Query 注入）。</para>
/// <para><b>路由族例外</b>：本域 3 支落在 <c>/wxa/business/novelreader/*</c>，其余 20 支小说端点落 <c>/wxa/book/*</c> ⇒ 同域两路由族，守卫须按族分别锁定，不得整域归一。</para>
/// <para><b>覆盖式写入</b>：<c>setpreviewsetting</c> 覆盖该作品的整份预览配置、<c>setrecmdnovel</c> 覆盖同一 <c>recmd_type</c> 下全部推荐（<c>book_id_list</c> 传空即清空）⇒ 均为全量写、非增量补丁，读回再改是必须流程。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "Novel", TokenManage = nameof(IMpAppManager))]
public interface IWxaNovelReaderService
{
    /// <summary>设置书本章节预览字数（每次设置覆盖同一 recmd_type 下的全部推荐）。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelSetPreviewSettingRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/preview/api_setpreviewsetting.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/business/novelreader/setpreviewsetting</c>；请求体是 <c>{"setting":{book_id, default_words, chapter_setting:[{chapter_index, words}]}}</c> 的<b>嵌套单对象</b>，与 <see cref="GetPreviewSettingAsync"/> 应答的 <c>setting</c> 同形（可直接读回改写）。</para>
    /// <para>未在 <c>chapter_setting</c> 出现的章节回落到 <c>default_words</c> ⇒ 全量覆盖语义，漏传即失去该章的定制字数。</para>
    /// </remarks>
    [Post("/wxa/business/novelreader/setpreviewsetting")]
    Task<WxaResponse> SetPreviewSettingAsync([Body] WxaNovelSetPreviewSettingRequest request, CancellationToken cancellationToken = default);

    /// <summary>获取书本章节预览字数设置。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelGetPreviewSettingRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>预览设置信息，见 <see cref="WxaNovelGetPreviewSettingResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/preview/api_getpreviewsetting.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/business/novelreader/getpreviewsetting</c>；请求体只有 <c>book_id</c>，应答为 <c>setting{book_id, default_words, chapter_setting[]}</c>。</para>
    /// </remarks>
    [Post("/wxa/business/novelreader/getpreviewsetting")]
    Task<WxaNovelGetPreviewSettingResponse> GetPreviewSettingAsync([Body] WxaNovelGetPreviewSettingRequest request, CancellationToken cancellationToken = default);

    /// <summary>设置阅读后推荐的其他小说（每次设置覆盖同一 recmd_type 下的全部推荐，最大 300 条）。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelSetRecmdNovelRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/other/api_novelreadersetrecmdnovel.html"/>（官方页在「其他」分区，slug 与路由末段 <c>setrecmdnovel</c> 不同形，勿按路由拼 slug）。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/business/novelreader/setrecmdnovel</c>；请求体 <c>{recmd_type, book_id_list}</c>，<c>book_id_list</c> 上限 300 条。</para>
    /// <para><b>按 <c>recmd_type</c> 分槽覆盖</b>：1-android（付费小说）/ 2-ios（全文免费小说）各一份列表 ⇒ 写一类不影响另一类；传空列表即清空该槽。</para>
    /// </remarks>
    [Post("/wxa/business/novelreader/setrecmdnovel")]
    Task<WxaResponse> SetRecmdNovelAsync([Body] WxaNovelSetRecmdNovelRequest request, CancellationToken cancellationToken = default);
}
