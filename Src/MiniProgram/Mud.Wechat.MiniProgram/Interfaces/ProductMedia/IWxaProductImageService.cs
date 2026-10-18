// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ProductMedia;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」·图片上传域（1 路由 2 形态：
/// 二进制流 multipart / 图片 URL JSON；路由 <c>/product/img/upload</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>同路由双请求形态</b>：官方以一个端点承载两种上传方式，由 Query <c>upload_type</c> 区分 ——
/// <c>0</c> 为 <c>multipart/form-data</c>（表单文件字段 <c>media</c>）、<c>1</c> 为 JSON
/// <c>{"img_url": …}</c> ⇒ 本接口按仓内既有「同路由多方法」形态拆成两支
/// （先例：小店线 <c>img_uploadv2</c>）。
/// </para>
/// <para>
/// <b>返回类型决定大小上限</b>：<c>resp_type=0</c> 回 <c>media_id</c> / <c>pay_media_id</c>、
/// 上限 2MB；<c>resp_type=1</c> 回图片链接、上限 10MB。
/// </para>
/// <para>
/// <b>链接限频、不可当图床</b>：<c>temp_img_url</c> 域名形如 <c>mmecimage.cn/p/{appid}/{imgKey}</c>，
/// 永久有效但按 appid 10000/min、imgKey 100/min 限频，超限 404，官方明示不得用于用户端大量展示。
/// </para>
/// <para>
/// <b>官方表与示例冲突（照录）</b>：<c>media_id</c> 参数表写 number、示例为字符串 ⇒ SDK 取
/// <c>string</c>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductMedia", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductImageService
{
    /// <summary>
    /// 上传图片——二进制流模式（<c>POST /product/img/upload?upload_type=0</c>，<c>multipart/form-data</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/register/uploadimg.html"/>。
    /// </summary>
    /// <param name="uploadType">上传方式（Query <c>upload_type</c>，必填）：本方法为二进制流模式，须传 <see cref="WxaProductImageUploadTypes.Binary"/>（0）。</param>
    /// <param name="respType">返回类型（Query <c>resp_type</c>，必填）：0 media_id / 1 图片链接，见 <see cref="WxaProductImageResponseTypes"/>。</param>
    /// <param name="height">图片高度，单位像素（<c>upload_type=0</c> 时必填）。</param>
    /// <param name="width">图片宽度，单位像素（<c>upload_type=0</c> 时必填）。</param>
    /// <param name="formData">
    /// multipart 表单内容，文件字段名须为官方 curl 示例的 <c>media</c>；由调用方构建
    /// <see cref="IFormContent"/> 实现。大小限制按 <paramref name="respType"/>：0 为 2MB、1 为 10MB。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>pic_file</c>（media_id / pay_media_id 或 temp_img_url），见 <see cref="WxaProductImageUploadResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>不声明 JSON 请求 DTO</b>：文件字段无法用 JSON 通路承载 ⇒ 守卫断言本方法恰有一个
    /// <c>[MultipartForm]</c> 参数、零 <c>[Body]</c> 参数。
    /// </para>
    /// <para>
    /// <b><c>upload_type</c> 必须由调用方显式传 0</b>（官方 Query 必填；SDK 不做方法内隐式改写，
    /// 与仓内小店线 <c>img_uploadv2</c> 双形态同口径）。
    /// </para>
    /// </remarks>
    [Post("/product/img/upload")]
    Task<WxaProductImageUploadResponse> UploadImageByBinaryAsync(
        [Query("upload_type")] int uploadType,
        [Query("resp_type")] int respType,
        [Query("height")] int height,
        [Query("width")] int width,
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传图片——图片 URL 模式（<c>POST /product/img/upload?upload_type=1</c>，JSON 体 <c>img_url</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/register/uploadimg.html"/>。
    /// </summary>
    /// <param name="uploadType">上传方式（Query <c>upload_type</c>，必填）：本方法为图片 URL 模式，须传 <see cref="WxaProductImageUploadTypes.Url"/>（1）。</param>
    /// <param name="respType">返回类型（Query <c>resp_type</c>，必填）：本模式通常取 1（图片链接），见 <see cref="WxaProductImageResponseTypes"/>。</param>
    /// <param name="request">请求体（<c>img_url</c>），见 <see cref="WxaProductImageUploadUrlRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>pic_file.temp_img_url</c>，见 <see cref="WxaProductImageUploadResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b><c>upload_type</c> 必须由调用方显式传 1</b>；<c>img_url</c> 在本模式必填，
    /// 官方注明 <c>resp_type=1</c> 时返回的链接永久有效、同一图片无需重复上传。
    /// </para>
    /// <para>
    /// <b>可直接使用的链接</b>：域名 <c>store.mp.video.tencent-cloud.com</c>、前缀
    /// <c>mmbizurl.cn/p/</c> 或 <c>mmecimage.cn/p/</c> 的图片返回 600201「无需转换、可直接使用」。
    /// </para>
    /// </remarks>
    [Post("/product/img/upload")]
    Task<WxaProductImageUploadResponse> UploadImageByUrlAsync(
        [Query("upload_type")] int uploadType,
        [Query("resp_type")] int respType,
        [Body] WxaProductImageUploadUrlRequest request,
        CancellationToken cancellationToken = default);
}
