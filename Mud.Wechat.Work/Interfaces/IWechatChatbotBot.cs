using Mud.Wechat.Work.DataModels;

namespace Mud.Wechat.Work;

/// <summary>
/// 机器人接口
/// </summary>
[HttpClientApi("https://api.weixin.qq.com", Timeout = 30, TokenManage = nameof(IWechatAppManager), RegistryGroupName = "Weixin")]
[Token(TokenType = TokenType.TenantAccessToken, InjectionMode = TokenInjectionMode.Path, Name = "TOKEN")]
public interface IWechatChatbotBot
{
    /// <summary>
    /// 设置批量导入问答-开放接口
    /// <para>异步调用 [POST] /batchimportskill/{TOKEN} 接口。</para>
    /// <para>
    /// REF: 请参照原SDK文件：<see href="https://developers.weixin.qq.com/doc/aispeech/confapi/deprecated/bot/batchimportskill.html"/>
    /// </para>
    /// </summary>
    /// <param name="request">批量导入技能请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量导入技能响应结果</returns>
    [Post("/batchimportskill/{TOKEN}")]
    Task<BatchImportSkillResponse> BatchImportSkillAsync(
        [Body(EnableEncrypt = true, EncryptSerializeType = SerializeType.Xml)] BatchImportSkillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 机器人发布-开放接口
    /// <para>异步调用 [POST] /publish/{TOKEN} 接口。</para>
    /// <para>
    /// REF: 请参照原SDK文件：<see href="https://developers.weixin.qq.com/doc/aispeech/confapi/deprecated/bot/publish.html"/>
    /// </para>
    /// </summary>
    /// <param name="request">发布请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>发布响应结果</returns>
    [Post("/publish/{TOKEN}")]
    Task<PublishResponse> PublishAsync(
        [Body(EnableEncrypt = true, EncryptSerializeType = SerializeType.Xml)] PublishRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 机器人发布进度查询-开放接口
    /// <para>异步调用 [POST] /publish_progress/{TOKEN} 接口。</para>
    /// <para>
    /// REF: 请参照原SDK文件：<see href="https://developers.weixin.qq.com/doc/aispeech/confapi/deprecated/bot/publish_progress.html"/>
    /// </para>
    /// </summary>
    /// <param name="request">发布进度请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>发布进度响应结果</returns>
    [Post("/publish_progress/{TOKEN}")]
    Task<PublishProgressResponse> PublishProgressAsync(
        [Body(EnableEncrypt = true, EncryptSerializeType = SerializeType.Xml)] PublishProgressRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置自动回复-开放接口
    /// <para>异步调用 [POST] /setautoreply/{TOKEN} 接口。</para>
    /// <para>
    /// REF: 请参照原SDK文件：<see href="https://developers.weixin.qq.com/doc/aispeech/confapi/deprecated/bot/setautoreply.html"/>
    /// </para>
    /// </summary>
    /// <param name="request">设置自动回复请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设置自动回复响应结果</returns>
    [Post("/setautoreply/{TOKEN}")]
    Task<SetAutoReplyResponse> SetAutoReplyAsync(
        [Body(EnableEncrypt = true, EncryptSerializeType = SerializeType.Xml)] SetAutoReplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户标签批量回传设置-开放接口
    /// <para>异步调用 [POST] /label/batchset/{TOKEN} 接口。</para>
    /// <para>
    /// REF: 请参照原SDK文件：<see href="https://developers.weixin.qq.com/doc/aispeech/confapi/deprecated/bot/batchsetlabel.html"/>
    /// </para>
    /// </summary>
    /// <param name="request">批量设置标签请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量设置标签响应结果</returns>
    [Post("/label/batchset/{TOKEN}")]
    Task<LabelBatchSetResponse> LabelBatchSetAsync(
        [Body(EnableEncrypt = true, EncryptSerializeType = SerializeType.Xml)] LabelBatchSetRequest request,
        CancellationToken cancellationToken = default);


}