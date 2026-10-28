// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Kf;

/// <summary>注册客服子商户请求体（<c>POST /cgi-bin/business/register</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>kf-mgnt/kf-management/api_registerbusiness.html</c>。</para>
/// <para>
/// <b>子商户形态（官方原文）</b>：一个小程序账号可为平台内的商户创建多个子商户账号，创建后
/// 在小程序客服组件唤起子商户单独的会话；注册成功后以 <c>mch_id</c> 作为唯一标识参与后续
/// <c>update / get / list</c> 操作。<c>brand_icon</c> 为<b>图片永久素材</b>的媒体 ID。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaBusinessRegisterRequest
{
    /// <summary>商户号（<c>mch_id</c>，必填；子商户唯一标识，后续更新 / 查询均以其定位）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>品牌名称（<c>brand_name</c>，必填）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>品牌图标（<c>brand_icon</c>，必填；图片永久素材媒体 ID）。</summary>
    [JsonPropertyName("brand_icon")]
    public string? BrandIcon { get; set; }
}
