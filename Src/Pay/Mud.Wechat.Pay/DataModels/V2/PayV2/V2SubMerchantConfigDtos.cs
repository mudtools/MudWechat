// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 子商户支付目录新增（<c>/secapi/mch/addsubdevconfig</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>本页请求面只有 5 行</b>：<c>appid</c>/<c>mch_id</c>/<c>sub_mch_id</c>/<c>jsapi_path</c>/<c>sign</c> ——
/// <b>没有</b> <c>nonce_str</c>、<b>没有</b> <c>sign_type</c>（官方页全文无该二字），
/// 与 <see cref="V2RecommendConfigAddRequest"/> 是同一域里的两种签名面，勿共用基类。</para>
/// <para><b>只增不改</b>：官方原文「API只支持新增配置，不支持修改，如银行需要修改请先登录微信服务商后台手工删除后重新配置」
/// ⇒ 本 SDK 不提供删除端点（官方无该 HTTP 面），配置错了只能回后台。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SubDevConfigAddRequest
{
    /// <summary>服务商的公众账号 ID。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>服务商的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>服务商报备的小微商户识别码。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>
    /// JSAPI 支付授权目录（须符合 URI 规范）。<b>每次只添加一个目录</b>，
    /// 每个商户最多 5 个（官方硬上限，第 6 次新增即失败）。
    /// </summary>
    [JsonPropertyName("jsapi_path")] public string? JsapiPath { get; set; }

    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 子商户支付目录查询（<c>/secapi/mch/querysubdevconfig</c>）请求体。
/// </summary>
/// <remarks>
/// <para>请求面 4 行（<c>appid</c>/<c>mch_id</c>/<c>sub_mch_id</c>/<c>sign</c>）：
/// 比新增少一个 <c>jsapi_path</c>，同样<b>没有</b> <c>nonce_str</c> 与 <c>sign_type</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SubDevConfigQueryRequest
{
    /// <summary>服务商的公众账号 ID。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>服务商的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>服务商报备的小微商户识别码。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 子商户支付目录<b>新增</b>应答：官方只回通信/业务两级标识 + 需验签的 <c>sign</c>。
/// </summary>
/// <remarks>
/// <para><b>应答 <c>sign</c> 必填</b>：官方描述「微信返回的签名，银行服务商需验证签名」⇒
/// 本线的证书收发器对标了 <c>sign</c> 的应答一律做验签，不做「可选校验」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SubDevConfigAddResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息，非空时为错误原因（签名失败、参数格式校验错误等）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果：SUCCESS / FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>微信返回的签名（官方必填，须验签）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
}

/// <summary>
/// APIv2 子商户支付目录<b>查询</b>应答：在信封之外多两个<b>值为 JSON 字符串</b>的字段。
/// </summary>
/// <remarks>
/// <para><b>不是 XML 子树</b>：<c>jsapi_path_list</c> 与 <c>appid_config_list</c> 官方类型均为 String，
/// 内容才是 JSON 文本 ⇒ 本 SDK 原样交回字符串，不猜结构（与 <see cref="V2ViolationRecordQueryResponse.ViolationRecords"/>
/// 同一处置口径）。</para>
/// <para><b>官方示例里的两处陷阱</b>：① 示例中 <c>appid_config_list</c> 的首个元素把键印成了
/// <c>" subscribe_appid "</c>（带空格），那是官方示例的笔误、不是字段名；② 说明文字用大写
/// <c>NULL</c> 作「不区分绑定」的哨兵，示例却给小写字符串 <c>"null"</c> ⇒ 解析侧须同时容忍两种写法。</para>
/// <para><b>上限</b>：支付目录最多返回 5 个（与新增侧的每商户 5 个对应）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SubDevConfigQueryResponse : V2SubDevConfigAddResponse
{
    /// <summary>
    /// 授权目录列表原文，JSON 形态 <c>{"jsapi_path_list":["http://…/","…"]}</c>，最多 5 个。
    /// </summary>
    [JsonPropertyName("jsapi_path_list")] public string? JsapiPathList { get; set; }

    /// <summary>
    /// 关联 APPID 与关注关系原文，JSON 形态 <c>{"appid_config_list":[{"sub_appid":"…","subscribe_appid":"…"}]}</c>；
    /// <c>sub_appid</c> 为绑定的公众号/小程序/APP 的 APPID，<c>subscribe_appid</c> 为推荐关注公众号的 APPID，
    /// 「不区分绑定」时该记录以 <c>sub_appid=NULL</c>（或示例里的 <c>"null"</c>）返回默认推荐关注的 APPID。
    /// </summary>
    [JsonPropertyName("appid_config_list")] public string? AppidConfigList { get; set; }
}

/// <summary>
/// APIv2 子商户关注配置新增（<c>/secapi/mkt/addrecommendconf</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>路径段是 <c>mkt</c> 不是 <c>mch</c></b>：与同域的
/// <see cref="V2SubDevConfigAddRequest"/>（<c>/secapi/mch/addsubdevconfig</c>）只差这一段，是最易写错的一处。</para>
/// <para><b>本支的签名面与同域另两支不同</b>：<c>nonce_str</c> 必填、<c>sign_type</c> 必填且
/// 官方原文「暂只支持HMAC-SHA256算法。该字段需参与签名sign的计算。」⇒ <see cref="SignType"/>
/// 的默认值 <c>HMAC-SHA256</c> 属契约面（改动即打红），清空它会把报文降成 MD5 而被官方拒。</para>
/// <para><b>本支请求面没有 <c>appid</c></b>（官方表就 8 行，主体标识只有 <c>mch_id</c> + <c>sub_mch_id</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2RecommendConfigAddRequest
{
    /// <summary>服务商商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>服务商报备的小微商户识别码。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>
    /// 绑定的服务商公众号/小程序/APP 支付 APPID。
    /// <b>必填</b>：官方原文「如果没有填 sub_appid，此处请填值 NULL」⇒ 无绑定关系时要上传
    /// 四字符大写哨兵 <c>NULL</c>，既不能省略也不能填空串。
    /// </summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }

    /// <summary>推荐关注的服务商公众号 APPID（与 <see cref="ReceiptAppId"/> 按官方「二选一」择一上传）。</summary>
    [JsonPropertyName("subscribe_appid")] public string? SubscribeAppId { get; set; }

    /// <summary>
    /// 支付凭证推荐小程序 appid：须为通过微信认证、且认证主体与服务商主体一致的小程序。
    /// </summary>
    [JsonPropertyName("receipt_appid")] public string? ReceiptAppId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型，官方「暂只支持 HMAC-SHA256」且该字段参与 <c>sign</c> 计算；默认值即契约面。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>APIv2 子商户关注配置新增应答（只回两级标识 + 需验签的 <c>sign</c>）。</summary>
/// <remarks>
/// <para><b>应答 <c>sign</c> 官方类型 String(64)</b>（与 HMAC-SHA256 摘要长度一致），
/// 而请求侧 <c>sign</c> 亦标 String(64) 却给了 32 位示例 —— 长度以本支的算法为准，字段名照抄。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2RecommendConfigAddResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息，非空时为错误原因（只支持 POST 方法、XML 格式错误、未使用 UTF-8 编码等）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果：SUCCESS / FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>微信返回的签名（官方必填，须验签）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
}
