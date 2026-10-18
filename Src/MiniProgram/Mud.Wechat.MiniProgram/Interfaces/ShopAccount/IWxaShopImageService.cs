// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopAccount;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 · 图片上传」域 SDK（1 端点：上传图片；
/// 官方 minishopopencomponent2 分区 → public 子目录「接入商品前必需接口」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>；
/// 官方本页无权限集编号、无第三方代调用与云调用声明（空缺留档）。
/// </para>
/// <para>
/// <b>本域唯一的非 JSON 通路</b>：官方 <c>Content-Type: multipart/form-data</c>、请求示例为
/// <c>curl -F media=@test.jpg "…/shop/img/upload?access_token=xxx" -F resp_type=0</c> ⇒ 走声明式
/// <c>[MultipartForm] IFormContent</c>（仓内既有形态：<c>IWxaVodUploadService</c> 单文件上传、
/// 公众号素材、企业微信临时素材），<b>不声明 JSON 请求 DTO</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopAccount", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopImageService
{
    /// <summary>
    /// 上传图片（<c>POST /shop/img/upload</c>，<c>multipart/form-data</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/public/upload_img.html"/>。
    /// </summary>
    /// <param name="formData">
    /// multipart 表单内容（由调用方构建 <see cref="IFormContent"/> 实现并标 <c>[MultipartForm]</c>，
    /// 生成器对该标注走 <see cref="IFormContent.ToHttpContentAsync"/> 通路）。官方表单字段须照原文：
    /// <c>media</c>（官方 curl 示例的文件字段）、<c>resp_type</c>（<b>必填</b>，number：
    /// <c>0</c> media_id（官方原文「目前只用于品牌申请品牌和类目」）/ <c>1</c> 返回链接 / <c>2</c> 返回微信支付 media_id）、
    /// <c>upload_type</c>（<b>必填</b>，number：<c>0</c> 图片流（<c>resp_type = 2</c> 时需指定 <c>filename</c>）/
    /// <c>1</c> 图片 url）、<c>img_url</c>（选填，<c>upload_type = 1</c> 时必传）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>图片信息（<c>img_info</c> 一层包装），见 <see cref="WxaShopImageUploadResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + <c>multipart/form-data</c>；官方本页无端点级频率数字（唯一数字型约束是
    /// <b>图片大小限制 2MB</b>，属规模限制，与频率分列）。</para>
    /// <para><b>不声明请求体 DTO</b>：官方请求侧是表单而非 JSON，JSON DTO 通路无法承载文件字段
    /// ⇒ 守卫 SHOPA3 断言本端点恰有一个 <c>[MultipartForm]</c> 参数、零 <c>[Body]</c> 参数。</para>
    /// <para><b>两支必填枚举 + 一支条件必填</b>（官方参数表原文）：<c>resp_type</c> 与 <c>upload_type</c> 均必填，
    /// <c>img_url</c> 在 <c>upload_type = 1</c> 时必传，<c>resp_type = 2</c> 且 <c>upload_type = 0</c> 时官方原文要求「需指定 filename」
    /// ⇒ 三者的组合合法性由宿主保证，SDK 不做校验。</para>
    /// <para><b>「临时链接 / 永久链接」的官方双表述</b>（同页并存，照录留档）：一处写「图片需使用图片上传接口换取
    /// 临时链接用于上传，上传后微信侧会转为永久链接。临时链接在微信侧永久有效，商家侧可以存储临时链接，
    /// 避免重复换取链接」，另一处给 <c>temp_img_url</c> 注「有访问频率限制，<b>不可用作图床</b>，后果自负」⇒
    /// 可缓存复用，但不可当静态资源分发。</para>
    /// <para><b>下游硬约束</b>：本端点产出的 <c>media_id</c> / 链接正是两支提审端点的资质图片位所需；
    /// 官方另给组件图片域名白名单 <c>store.mp.video.tencent-cloud.com</c>、<c>mmbizurl.cn</c>、<c>mmecimage.cn/p/</c>，
    /// 自有 url 打不开会直接导致审核驳回。</para>
    /// <para><c>media_id</c>（官方示例前缀 <c>wxashop_</c>）与 <c>pay_media_id</c> 分属两套体系，不得互换提交。</para>
    /// </remarks>
    [Post("/shop/img/upload")]
    Task<WxaShopImageUploadResponse> UploadImageAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);
}
