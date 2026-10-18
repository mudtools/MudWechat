// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 获取城市服务限定页面链接请求体（<c>POST /cityservice/getservicepath</c>）。
/// </summary>
/// <remarks>
/// <para><b>条件必填矩阵</b>：<c>city_name</c> 在 page_type 1/3/5 必填；<c>content_name</c> 在 3 必填；
/// <c>ext_params</c> 在 5 必填；<c>service_id</c> 与 <c>params</c> 在 0 必填；
/// <c>need_path_type</c> / <c>device_type</c> 仅获取 H5 URL 时填写（OpenSDK 不填）。</para>
/// <para><b><c>params</c> 是字符串化的 JSON 数组</b>：每项 <c>{key,value}</c>，最多 10 个、key 不重复、
/// 值必须是字符串；<c>key=city</c> 表示用户所在城市。注意它是 <c>string</c> 而非对象数组。</para>
/// <para><b>官方示例格式瑕疵（照录）</b>：示例使用单引号 <c>'广州'</c>、带空格的键 <c>" params "</c>、
/// 以及多余引号（<c>"xxx”"</c>）⇒ 示例不可直接解析，本类按参数表建模。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServiceGetServicePathRequest
{
    /// <summary>获取城市服务路径类型（必填），见 <see cref="WxaCityServicePageTypes"/>。</summary>
    [JsonPropertyName("page_type")]
    public int? PageType { get; set; }

    /// <summary>跳转来源渠道（必填），见 <see cref="WxaCityServiceSrcChannels"/>。</summary>
    [JsonPropertyName("src_channel")]
    public int? SrcChannel { get; set; }

    /// <summary>获取 h5 URL 时填 <c>1</c>；OpenSDK 不需要填写。</summary>
    [JsonPropertyName("need_path_type")]
    public int? NeedPathType { get; set; }

    /// <summary>获取 h5 URL 时填 <c>2</c>；OpenSDK 不需要填写。</summary>
    [JsonPropertyName("device_type")]
    public int? DeviceType { get; set; }

    /// <summary>城市名称，page_type 为 1/3/5 时必填（以地级市为准）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>专题页名称，page_type 为 3 时必填。</summary>
    [JsonPropertyName("content_name")]
    public string? ContentName { get; set; }

    /// <summary>附加参数（含关键字等），page_type 为 5 时必填。</summary>
    [JsonPropertyName("ext_params")]
    public List<WxaCityServicePathExtParam>? ExtParams { get; set; }

    /// <summary>服务 id，page_type 为 0 时必填。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>透传参数（JSON 数组字符串），page_type 为 0 时必填；最多 10 个、key 不重复、值须为字符串。</summary>
    [JsonPropertyName("params")]
    public string? Params { get; set; }

    /// <summary>用户所在城市 id（需与腾讯内部同一套编码），page_type 为 0 时可传。</summary>
    [JsonPropertyName("city_id")]
    public string? CityId { get; set; }
}
