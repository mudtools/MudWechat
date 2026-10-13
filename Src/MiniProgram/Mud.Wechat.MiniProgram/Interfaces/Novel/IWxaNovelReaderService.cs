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
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/preview/api_setpreviewsetting.html"/> 等。</para>
/// <para>权限集 169；不支持云调用；支持第三方平台代商家调用。</para>
/// <para>预览设置与读后推荐属于阅读器插件的运营能力。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "Novel", TokenManage = nameof(IMpAppManager))]
public interface IWxaNovelReaderService
{
    /// <summary>设置书本章节预览字数（每次设置覆盖同一 recmd_type 下的全部推荐）。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [Post("/wxa/business/novelreader/setpreviewsetting")]
    Task<WxaResponse> SetPreviewSettingAsync([Body] WxaNovelSetPreviewSettingRequest request, CancellationToken cancellationToken = default);

    /// <summary>获取书本章节预览字数设置。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>预览设置信息。</returns>
    [Post("/wxa/business/novelreader/getpreviewsetting")]
    Task<WxaNovelGetPreviewSettingResponse> GetPreviewSettingAsync([Body] WxaNovelGetPreviewSettingRequest request, CancellationToken cancellationToken = default);

    /// <summary>设置阅读后推荐的其他小说（每次设置覆盖同一 recmd_type 下的全部推荐，最大 300 条）。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [Post("/wxa/business/novelreader/setrecmdnovel")]
    Task<WxaResponse> SetRecmdNovelAsync([Body] WxaNovelSetRecmdNovelRequest request, CancellationToken cancellationToken = default);
}
