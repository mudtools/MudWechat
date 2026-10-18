// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 上传图片结果内容体（官方 <c>/shop/img/upload</c> 回包 <c>img_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>三支各对应一种 <c>resp_type</c></b>（官方请求表：<c>0</c> media_id、<c>1</c> 返回链接、
/// <c>2</c> 返回微信支付 media_id）⇒ 按请求侧选择的类型回填对应一支，其余为空。</para>
/// <para><b>「临时链接」与「永久」的关系（官方原文两条并存）</b>：
/// 「图片需使用图片上传接口换取<b>临时链接</b>用于上传，上传后微信侧会转为<b>永久链接</b>。
/// 临时链接在微信侧永久有效，商家侧可以存储临时链接，避免重复换取链接」——同一段里
/// <c>temp_img_url</c> 又标注「有访问频率限制，<b>不可用作图床</b>，后果自负」⇒
/// 该链接可缓存复用，但不可当静态资源分发。</para>
/// <para><b>微信侧组件图片域名白名单（官方列三支域名）</b>：<c>store.mp.video.tencent-cloud.com</c>、
/// <c>mmbizurl.cn</c>、<c>mmecimage.cn/p/</c>——提审时传自有 url 必须能正常打开，否则审核驳回。</para>
/// <para><c>media_id</c> 官方示例前缀为 <c>wxashop_</c>，与微信支付侧 <c>pay_media_id</c> 是两个体系的 id，
/// 不得互换提交。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopImageInfo
{
    /// <summary>图片链接（<c>temp_img_url</c>，string；<c>resp_type = 1</c> 时给出。官方原文：有访问频率限制、不可用作图床）。</summary>
    [JsonPropertyName("temp_img_url")]
    public string? TempImgUrl { get; set; }

    /// <summary>开放平台 media_id（<c>media_id</c>，string；<c>resp_type = 0</c> 时给出，官方示例前缀 <c>wxashop_</c>；官方注明「目前只用于品牌申请品牌和类目」）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>微信支付 media_id（<c>pay_media_id</c>，string；<c>resp_type = 2</c> 时给出，属支付侧体系、与本线其余 media_id 不互换）。</summary>
    [JsonPropertyName("pay_media_id")]
    public string? PayMediaId { get; set; }
}
