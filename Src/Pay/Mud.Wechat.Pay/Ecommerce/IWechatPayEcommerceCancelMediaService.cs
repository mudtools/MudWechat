// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Ecommerce;

/// <summary>
/// 平台收付通「注销申请资料图片」上传服务（手工通道，<c>multipart/form-data</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691710"/>
/// （更新时间 2024.11.05，支持商户【普通服务商】【平台商户】）。
/// </para>
/// <para>
/// <b>为何不能声明式</b>：本端点是 <c>multipart/form-data</c>，非 JSON 载荷；SDK 需自行构建表单、
/// 计算 SHA-256 摘要、序列化 <c>meta</c> JSON 对象 ⇒ 与 <c>IWechatPayMerchantMediaService</c>、
/// <c>IWechatPayFapiaoFileService</c> 同款归属（依赖 <c>AddPayApp</c> 注册的 <c>IWechatPayHttpClient</c>）。
/// </para>
/// <para>
/// <b>表单字段名与本线其它上传通道不同</b>：本页是 <c>file</c> + <c>meta{file_name, file_digest}</c>，
/// 而商户媒体上传是 <c>meta{filename, sha256}</c> ⇒ 两个 meta 的键名<b>不通用</b>，
/// 照搬即官方 <c>PARAM_ERROR</c>（守卫 EC-X2 锁定键名）。
/// </para>
/// <para>
/// <b>它是注销申请的强制前置</b>：官方原文「需要先调用本接口上传相关的资料图片，获取图片ID后，
/// 再填写到注销申请请求中」⇒ <c>application_info.application_media_id</c> 必须来自本通道。
/// </para>
/// </remarks>
public interface IWechatPayEcommerceCancelMediaService
{
    /// <summary>
    /// 上传注销申请资料图片（<c>POST /v3/ecommerce/account/cancel-applications/media</c>）。
    /// </summary>
    /// <param name="content">图片二进制内容流。</param>
    /// <param name="fileName">文件名称：必须以 <c>JPG</c>/<c>BMP</c>/<c>PNG</c> 为后缀（官方 <c>meta.file_name</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>微信返回的媒体文件标识 ID（<c>media_id</c>，string(512)），填入注销申请的 <c>application_media_id</c>。</returns>
    /// <remarks>
    /// <para><b>官方限制</b>：媒体图片<b>只支持 JPG、BMP、PNG</b>，文件大小<b>不能超过 2M</b> ⇒
    /// 本方法在读取字节后本地即校验这两条（官方对应 400 <c>INVALID_REQUEST</c>
    /// 「图片内容过大」「图片Content-Type非法」），避免把大文件传到一半才失败。</para>
    /// <para><b>文件摘要</b>：<c>meta.file_digest</c> 是图片二进制的 SHA-256（官方注明「大小写均可」，
    /// 本实现统一小写十六进制）。</para>
    /// <para><b>页内无频率数字</b>；业务错误码 400 <c>PARAM_ERROR</c> / 400 <c>INVALID_REQUEST</c> /
    /// 401 <c>SIGN_ERROR</c> / 500 <c>SYSTEM_ERROR</c>。</para>
    /// </remarks>
    Task<string> UploadCancelProofImageAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default);
}
