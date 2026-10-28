// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingBank;

/// <summary>银行营销券码包上传任务响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466703"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBank")]
public class MarketingBankPackageTaskCreateResponse : WechatPayResponse
{
    /// <summary>【上传任务】 上传任务的主键，唯一定义此资源的标识（官方字段路径 <c>task_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>【号码包id】 号码包唯一标识符。可在微信支付商户平台创建号码包后获得（官方字段路径 <c>package_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("package_id")]
    public string? PackageId { get; set; }

    /// <summary>【文件名】 任务上传的文件名（官方字段路径 <c>filename</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    /// <summary>【创建上传任务的时间】 时间格式采用遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【上传任务最近一次更新的时间】 时间格式采用遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【任务状态】 上传任务的状态：PROCESSING - 处理中，FINISHED - 已完成。可选取值：`PROCESSING` 上传任务处理中，请等待 / `FINISHED` 上传任务已处理完成，任务上传的文件中的用户已导入到号码包中（官方字段路径 <c>status</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>【成功数】 匹配成功的协议号数。仅当任务状态是FINISHED时，该数据才有效（官方字段路径 <c>success_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("success_count")]
    public long? SuccessCount { get; set; }

    /// <summary>【失败数】 匹配失败的协议号数。仅当任务状态是FINISHED时，该数据才有效（官方字段路径 <c>fail_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("fail_count")]
    public long? FailCount { get; set; }

    /// <summary>【匹配成功的微信用户数】 匹配成功的微信用户数，不小于匹配成功的协议号数。当一张银行卡被用户绑定到不同微信号时，才可能出现一个协议号对应多个微信账户的情况。仅当任务状态是FINISHED时，该数据才有效（官方字段路径 <c>success_user_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("success_user_count")]
    public long? SuccessUserCount { get; set; }

    /// <summary>【银行类型】 银行类型，用于标识协议号所属的银行以及卡类型(信用卡或借记卡)。采用字符串类型的银行标识，值列表详见 银行类型（官方字段路径 <c>bank_type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

}

/// <summary>银行营销券码包上传任务列表响应·【上传任务列表】 符合条件的上传任务列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466699"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBank")]
public class MarketingBankPackageTaskQueryResponseDataItem
{
    /// <summary>【上传任务】 上传任务的主键，唯一定义此资源的标识（官方字段路径 <c>data.task_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>【号码包id】 号码包唯一标识符。可在微信支付商户平台创建号码包后获得（官方字段路径 <c>data.package_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("package_id")]
    public string? PackageId { get; set; }

    /// <summary>【文件名】 任务上传的文件名（官方字段路径 <c>data.filename</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    /// <summary>【创建上传任务的时间】 时间格式采用遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【上传任务最近一次更新的时间】 时间格式采用遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【任务状态】 上传任务的状态：PROCESSING - 处理中，FINISHED - 已完成。可选取值：`PROCESSING` 上传任务处理中，请等待 / `FINISHED` 上传任务已处理完成，任务上传的文件中的用户已导入到号码包中（官方字段路径 <c>data.status</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>【成功数】 匹配成功的协议号数。仅当任务状态是FINISHED时，该数据才有效（官方字段路径 <c>data.success_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("success_count")]
    public long? SuccessCount { get; set; }

    /// <summary>【失败数】 匹配失败的协议号数。仅当任务状态是FINISHED时，该数据才有效（官方字段路径 <c>data.fail_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("fail_count")]
    public long? FailCount { get; set; }

    /// <summary>【匹配成功的微信用户数】 匹配成功的微信用户数，不小于匹配成功的协议号数。当一张银行卡被用户绑定到不同微信号时，才可能出现一个协议号对应多个微信账户的情况。仅当任务状态是FINISHED时，该数据才有效（官方字段路径 <c>data.success_user_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("success_user_count")]
    public long? SuccessUserCount { get; set; }

    /// <summary>【银行类型】 银行类型，用于标识协议号所属的银行以及卡类型(信用卡或借记卡)。采用字符串类型的银行标识，值列表详见 银行类型（官方字段路径 <c>data.bank_type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

}

/// <summary>银行营销券码包上传任务列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466699"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBank")]
public class MarketingBankPackageTaskQueryResponse : WechatPayResponse
{
    /// <summary>【上传任务列表】 符合条件的上传任务列表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MarketingBankPackageTaskQueryResponseDataItem>? Data { get; set; }

    /// <summary>【总数量】 总数量（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【分页页码】 分页页码，页码从0开始（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

}
