// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

// ---------------------------------------------------------------- 申请二维码（POST /intp/marketcode/applycode）

/// <summary>
/// 申请二维码（<c>POST /intp/marketcode/applycode</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方注意事项原文：① 「<c>code_count</c> 参数必须是 <b>10000 的整数倍</b>，范围 <b>10000-20000000</b>」；
/// ② 「相同 <c>isv_application_id</c> 视为同一申请单」（⇒ 幂等键，重试不会重复生成）。
/// SDK <b>不做本地范围/倍数拦截</b>（沿用「越界由官方表达」的既存口径）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：本页错误码表把 <c>40002 invalid grant_type</c> 一并列出——该参数属
/// <c>getAccessToken</c> 接口，本接口请求体<b>无</b> <c>grant_type</c> 字段（误挂，照录不建模）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpApplyCodeRequest
{
    /// <summary>获取或设置申请码的数量（官方 <c>code_count</c>，必填；10000 的整数倍，范围 10000~20000000）。</summary>
    [JsonPropertyName("code_count")]
    public int CodeCount { get; set; }

    /// <summary>获取或设置外部单号（官方 <c>isv_application_id</c>，必填；相同值视为同一申请单 ⇒ 幂等键）。</summary>
    [JsonPropertyName("isv_application_id")]
    public string IsvApplicationId { get; set; } = string.Empty;
}

/// <summary>
/// 申请二维码（<c>POST /intp/marketcode/applycode</c>）响应。
/// </summary>
/// <remarks>
/// <b>响应为平级字段（无 <c>data</c> 包裹）</b>——本域 6 端点均为平级形态（与门店域的 <c>data</c> 包裹不同）。
/// 返回<b>不含二维码本体</b>，仅申请单号；取码须另调 <c>applycodequery</c> 与 <c>applycodedownload</c>。
/// 官方错误码：<c>0</c> / <c>40001</c> / <c>40002</c>（后两项见类型级 remarks 的误挂说明）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpApplyCodeResponse : MpResponse
{
    /// <summary>获取或设置申请单号（官方 <c>application_id</c>）。</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }
}

// ---------------------------------------------------------------- 查询二维码申请单（POST /intp/marketcode/applycodequery）

/// <summary>
/// 查询二维码申请单（<c>POST /intp/marketcode/applycodequery</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方必填性矛盾（照录，SDK 以可空承载）</b>：官方字段表把 <c>application_id</c> 与
/// <c>isv_application_id</c> <b>都标为必填</b>，但官方注意事项原文为「需要通过 <c>application_id</c>
/// <b>或</b> <c>isv_application_id</c> 查询申请单状态」⇒ 二者实为<b>二选一</b>。SDK 不本地校验。
/// </para>
/// <para>
/// <b>类型口径说明</b>：官方字段表标 <c>application_id</c> 为 <c>number</c>，而官方<b>请求示例</b>写作字符串
/// <c>"581865877"</c>。按本 SDK 冲突处置规则，标量冲突的「取示例」仅适用于<b>返回示例（真机报文）</b>；
/// <b>请求示例</b>由文档作者手写且渲染常加引号，不构成口径依据 ⇒ 请求侧取<b>字段表</b>（数值）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpApplyCodeQueryRequest
{
    /// <summary>获取或设置申请单号（官方 <c>application_id</c>；与 <see cref="IsvApplicationId"/> 二选一）。</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }

    /// <summary>获取或设置外部单号（官方 <c>isv_application_id</c>；与 <see cref="ApplicationId"/> 二选一）。</summary>
    [JsonPropertyName("isv_application_id")]
    public string? IsvApplicationId { get; set; }
}

/// <summary>
/// 查询二维码申请单（<c>POST /intp/marketcode/applycodequery</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（平级）：<c>status</c>（申请单状态）/ <c>application_id</c> / <c>isv_application_id</c> /
/// <c>code_generate_list</c>（二维码信息数组）/ <c>create_time</c> / <c>update_time</c>。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：<c>status</c> <b>无枚举表</b>（仅返回示例出现 <c>FINISH</c>，SDK 只登记该示例值，
/// 见 <c>MpCodeApplyStatuses</c>）；<c>create_time</c>/<c>update_time</c> 未说明单位与格式。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeApplyQueryResponse : MpResponse
{
    /// <summary>获取或设置申请单状态（官方 <c>status</c>；官方未给枚举表，示例为 <c>FINISH</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置申请单号（官方 <c>application_id</c>）。</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }

    /// <summary>获取或设置外部单号（官方 <c>isv_application_id</c>）。</summary>
    [JsonPropertyName("isv_application_id")]
    public string? IsvApplicationId { get; set; }

    /// <summary>获取或设置二维码信息（官方 <c>code_generate_list</c>；码段起止，供下载与激活使用）。</summary>
    [JsonPropertyName("code_generate_list")]
    public List<MpCodeRange>? CodeGenerateList { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>；官方未说明单位与格式）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>；官方未说明单位与格式）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

/// <summary>
/// 二维码码段（官方 <c>code_generate_list[]</c> 与下载/激活端点的 <c>code_start</c>/<c>code_end</c> 共用语义）。
/// </summary>
/// <remarks>官方字段表：<c>code_start</c> 开始位置 / <c>code_end</c> 结束位置。</remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeRange
{
    /// <summary>获取或设置开始位置（官方 <c>code_start</c>）。</summary>
    [JsonPropertyName("code_start")]
    public int? CodeStart { get; set; }

    /// <summary>获取或设置结束位置（官方 <c>code_end</c>）。</summary>
    [JsonPropertyName("code_end")]
    public int? CodeEnd { get; set; }
}

// ---------------------------------------------------------------- 下载二维码包（POST /intp/marketcode/applycodedownload）

/// <summary>
/// 下载二维码包（<c>POST /intp/marketcode/applycodedownload</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>application_id</c> / <c>code_start</c> / <c>code_end</c>（后两者「来自查询二维码申请接口」）。
/// </para>
/// <para>
/// 官方注意事项原文：「<b>下载前需确保申请单状态为 FINISH</b>」——SDK 不编排该前置校验（宿主自行编排）。
/// 官方文档缺陷（照录）：<c>code_start</c>/<c>code_end</c> 未说明取值范围、最大下载数量、是否含边界值。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeDownloadRequest
{
    /// <summary>获取或设置申请单号（官方 <c>application_id</c>，必填）。</summary>
    [JsonPropertyName("application_id")]
    public long ApplicationId { get; set; }

    /// <summary>获取或设置开始位置（官方 <c>code_start</c>，必填；来自 <c>applycodequery</c> 的码段）。</summary>
    [JsonPropertyName("code_start")]
    public int CodeStart { get; set; }

    /// <summary>获取或设置结束位置（官方 <c>code_end</c>，必填；来自 <c>applycodequery</c> 的码段）。</summary>
    [JsonPropertyName("code_end")]
    public int CodeEnd { get; set; }
}

/// <summary>
/// 下载二维码包（<c>POST /intp/marketcode/applycodedownload</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档缺陷（照录，SDK 的建模裁决）</b>：官方「返回参数」表把 <c>buffer</c> 的类型列为
/// 「<b>formdata 文件 buffer</b>」（<b>非合法 JSON 类型，无法据以建模</b>），而同页返回示例为
/// <b>JSON 包裹</b>的 <c>{"errcode":…,"errmsg":…,"buffer":"…"}</c>。SDK 按<b>示例</b>建模为
/// <see cref="Buffer"/> 字符串（base64），符合「表类型列无法建模 ⇒ 取示例」的处置。
/// </para>
/// <para>
/// <b>解码链（官方原文，SDK 不代解码）</b>：「需先 <b>base64 decode</b>，再做<b>解密</b>操作」；
/// 官方注「解密参见 3.1」——<b>该章节在本页不存在</b>（章节跳至 4/5 节），解密算法与密钥来源官方未给出
/// ⇒ SDK <b>不实现</b>解码/解密，由宿主按官方专项文档处理。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeDownloadResponse : MpResponse
{
    /// <summary>获取或设置二维码包数据（官方 <c>buffer</c>；base64 编码，须 decode 后解密；SDK 不代解码）。</summary>
    [JsonPropertyName("buffer")]
    public string? Buffer { get; set; }
}

// ---------------------------------------------------------------- 激活二维码（POST /intp/marketcode/codeactive）

/// <summary>
/// 激活二维码（<c>POST /intp/marketcode/codeactive</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>application_id</c> / <c>activity_name</c> / <c>product_brand</c> / <c>product_title</c> /
/// <c>product_code</c> / <c>wxa_appid</c> / <c>wxa_path</c> / <c>code_start</c> / <c>code_end</c> 必填；
/// <c>wxa_type</c> 可选（默认 <c>0</c> 正式版，见 <c>MpCodeWxaTypes</c>）。
/// </para>
/// <para>
/// 官方原文：<c>activity_name</c>/<c>product_brand</c>/<c>product_title</c> 均为「数据分析…区分依据，<b>请规范命名</b>」；
/// <c>product_code</c> 为「EAN 商品条码，请规范填写」。
/// </para>
/// <para>
/// <b>激活范围（官方原文）</b>：<c>code_start</c>/<c>code_end</c> 为<b>闭区间</b>（两端「包含该值」）。
/// 官方文档缺陷（照录）：未给出合法取值范围、是否允许倒序、最大跨度与申请单容量的对应关系；
/// 且说明举例上限 <c>9999</c> 与代码示例的上限 <c>200</c> 不一致。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeActiveRequest
{
    /// <summary>获取或设置申请单号（官方 <c>application_id</c>，必填）。</summary>
    [JsonPropertyName("application_id")]
    public long ApplicationId { get; set; }

    /// <summary>获取或设置活动名称（官方 <c>activity_name</c>，必填；数据分析活动区分依据）。</summary>
    [JsonPropertyName("activity_name")]
    public string ActivityName { get; set; } = string.Empty;

    /// <summary>获取或设置商品品牌（官方 <c>product_brand</c>，必填；数据分析品牌区分依据）。</summary>
    [JsonPropertyName("product_brand")]
    public string ProductBrand { get; set; } = string.Empty;

    /// <summary>获取或设置商品标题（官方 <c>product_title</c>，必填；数据分析商品区分依据）。</summary>
    [JsonPropertyName("product_title")]
    public string ProductTitle { get; set; } = string.Empty;

    /// <summary>获取或设置商品条码（官方 <c>product_code</c>，必填；EAN 商品条码）。</summary>
    [JsonPropertyName("product_code")]
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>获取或设置扫码跳转的小程序 appid（官方 <c>wxa_appid</c>，必填）。</summary>
    [JsonPropertyName("wxa_appid")]
    public string WxaAppId { get; set; } = string.Empty;

    /// <summary>获取或设置扫码跳转的小程序 path（官方 <c>wxa_path</c>，必填）。</summary>
    [JsonPropertyName("wxa_path")]
    public string WxaPath { get; set; } = string.Empty;

    /// <summary>获取或设置小程序版本（官方 <c>wxa_type</c>，可选；默认 <c>0</c> 正式版，见 <c>MpCodeWxaTypes</c>）。</summary>
    [JsonPropertyName("wxa_type")]
    public int? WxaType { get; set; }

    /// <summary>获取或设置激活码段起始位（官方 <c>code_start</c>，必填；<b>包含该值</b>）。</summary>
    [JsonPropertyName("code_start")]
    public int CodeStart { get; set; }

    /// <summary>获取或设置激活码段结束位（官方 <c>code_end</c>，必填；<b>包含该值</b>）。</summary>
    [JsonPropertyName("code_end")]
    public int CodeEnd { get; set; }
}

// ---------------------------------------------------------------- 查询激活状态 / CODE_TICKET 换 CODE（共用响应）

/// <summary>
/// 查询二维码激活状态（<c>POST /intp/marketcode/codeactivequery</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方注意事项原文：「支持通过 <c>application_id</c>+<c>code_index</c> <b>或</b> <c>code</c>/URL 两种查询方式」
/// ⇒ 两条路径互斥。官方字段表标注：<c>application_id</c> 非必填、<c>code_index</c>「传入
/// <c>application_id</c> 时必填」、<c>code_url</c> 与 <c>code</c> 二选一。SDK 全部可空、不本地校验
/// （官方文档缺陷：两条路径的优先级与互斥关系未闭合说明，照录）。
/// </para>
/// <para>
/// <b>类型口径（关键）</b>：官方请求字段表把 <c>code</c> 标为 <c>number</c>，但同表明文描述为
/// 「<b>九位的字符串原始码</b>」、且<b>返回体字段表</b>同名字段标 <c>string</c> —— 三处证据中两处为字符串，
/// 且「原始码」可能有<b>前导零</b>（数值类型会丢）⇒ 建模为 <c>string</c>（请求表的 number 标注为误标，照录）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeActiveQueryRequest
{
    /// <summary>获取或设置申请单号（官方 <c>application_id</c>；配合 <see cref="CodeIndex"/> 使用）。</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }

    /// <summary>获取或设置该码在批次中的偏移量（官方 <c>code_index</c>；传入 <c>application_id</c> 时必填）。</summary>
    [JsonPropertyName("code_index")]
    public int? CodeIndex { get; set; }

    /// <summary>获取或设置普通码字符（官方 <c>code_url</c>；28 位，与 <see cref="Code"/> 二选一）。</summary>
    [JsonPropertyName("code_url")]
    public string? CodeUrl { get; set; }

    /// <summary>获取或设置原始码（官方 <c>code</c>；九位字符串原始码，与 <see cref="CodeUrl"/> 二选一）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}

/// <summary>
/// 「原始码 + 激活信息」响应（<c>codeactivequery</c> 与 <c>tickettocode</c> <b>共用</b>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方两页的响应字段表<b>语义完全相同</b>（均为「原始码数据，并返回对应的激活信息」），字段集仅相差
/// <c>wxa_type</c> 一项（<c>tickettocode</c> 页未列，<b>疑官方漏列</b>）；且 STJ 对未声明字段自动忽略
/// ⇒ SDK 以<b>超集共用</b>本 DTO（比复制 10 个字段更优；差异照录于此）。
/// </para>
/// <para>
/// <b>存在性冲突取并集</b>：两页返回<b>示例</b>均出现 <c>product_code</c>，但两页字段表<b>均未收录</b>
/// ⇒ SDK 保留 <see cref="ProductCode"/>。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：<c>codeactivequery</c> 页返回示例 JSON 缺逗号（非法 JSON）；
/// <c>tickettocode</c> 页示例使用中文全角逗号；两页 <c>code_end</c> 示例值（<c>200</c>）与字段说明举例（<c>9999</c>）不一致。
/// </para>
/// <para>官方错误码：<c>40001</c>（两页错误码表均仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeInfoResponse : MpResponse
{
    /// <summary>获取或设置原始码（官方 <c>code</c>；九位字符串原始码，返回码数据与对应激活信息）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置申请单号（官方 <c>application_id</c>）。</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }

    /// <summary>获取或设置外部单号（官方 <c>isv_application_id</c>）。</summary>
    [JsonPropertyName("isv_application_id")]
    public string? IsvApplicationId { get; set; }

    /// <summary>获取或设置活动名称（官方 <c>activity_name</c>）。</summary>
    [JsonPropertyName("activity_name")]
    public string? ActivityName { get; set; }

    /// <summary>获取或设置商品品牌（官方 <c>product_brand</c>）。</summary>
    [JsonPropertyName("product_brand")]
    public string? ProductBrand { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>product_title</c>）。</summary>
    [JsonPropertyName("product_title")]
    public string? ProductTitle { get; set; }

    /// <summary>获取或设置商品条码（官方返回示例字段 <c>product_code</c>；两页字段表均未收录，照录）。</summary>
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; set; }

    /// <summary>获取或设置小程序 appid（官方 <c>wxa_appid</c>）。</summary>
    [JsonPropertyName("wxa_appid")]
    public string? WxaAppId { get; set; }

    /// <summary>获取或设置小程序 path（官方 <c>wxa_path</c>）。</summary>
    [JsonPropertyName("wxa_path")]
    public string? WxaPath { get; set; }

    /// <summary>获取或设置小程序版本（官方 <c>wxa_type</c>；<c>tickettocode</c> 页字段表未列，疑漏）。</summary>
    [JsonPropertyName("wxa_type")]
    public int? WxaType { get; set; }

    /// <summary>获取或设置激活码段起始位（官方 <c>code_start</c>；包含该值）。</summary>
    [JsonPropertyName("code_start")]
    public int? CodeStart { get; set; }

    /// <summary>获取或设置激活码段结束位（官方 <c>code_end</c>；包含该值）。</summary>
    [JsonPropertyName("code_end")]
    public int? CodeEnd { get; set; }
}

/// <summary>
/// CODE_TICKET 换 CODE（<c>POST /intp/marketcode/tickettocode</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>openid</c>（用户 openid）/ <c>code_ticket</c>（跳转时带上的临时票据，用于换取正式营销码）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：① 本页<b>无 Query 参数表</b>（其余 5 页均列 <c>access_token</c>）⇒
/// SDK 按同域契约仍以 <b>Query 注入 <c>access_token</c></b>（第三方代调用时为 <c>authorizer_access_token</c>）；
/// ② 接口概述称 <c>code_ticket</c> 为「用户<b>扫码</b>获得」，字段说明称「<b>跳转</b>时带上」，获取途径描述不一致。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpTicketToCodeRequest
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置临时票据（官方 <c>code_ticket</c>，必填；用户扫码/跳转携带，用于换取正式营销码）。</summary>
    [JsonPropertyName("code_ticket")]
    public string CodeTicket { get; set; } = string.Empty;
}
