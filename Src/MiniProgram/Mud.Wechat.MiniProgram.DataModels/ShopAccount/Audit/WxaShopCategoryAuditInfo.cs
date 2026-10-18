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
/// 类目审核的类目定位与资质材料（官方 <c>audit_req.category_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>键名是 <c>level1</c> / <c>level2</c> / <c>level3</c></b>，与本域其余三套类目键名
/// （<c>*_cat_id</c>、<c>third_cat_id</c>、<c>*_category_id</c>）都不同 ⇒ 照官方原文，不得改写（守卫 SHOPA5）。</para>
/// <para><b>官方类型列与说明列矛盾</b>：三支类型列写 <c>uint32</c>，说明列写「字符类型，最长不超过 10」⇒
/// 本仓按类型列建为可空数值（官方请求示例亦为裸数字 <c>7419</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopCategoryAuditInfo
{
    /// <summary>一级类目 ID（<c>level1</c>，官方类型列 uint32，必填，示例 7419）。</summary>
    [JsonPropertyName("level1")]
    public long? Level1 { get; set; }

    /// <summary>二级类目 ID（<c>level2</c>，官方类型列 uint32，必填，示例 7439）。</summary>
    [JsonPropertyName("level2")]
    public long? Level2 { get; set; }

    /// <summary>三级类目 ID（<c>level3</c>，官方类型列 uint32，必填，示例 7448；取自 <c>/shop/cat/get</c> 的 <c>third_cat_id</c>）。</summary>
    [JsonPropertyName("level3")]
    public long? Level3 { get; set; }

    /// <summary>资质材料（<c>certificate</c>，string 数组，<b>必填</b>，官方原文「最多不超过 10 张」）。</summary>
    [JsonPropertyName("certificate")]
    public List<string>? Certificate { get; set; }
}
