// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Capital;

/// <summary>
/// 查询银行列表（对公 / 对私两支共用）响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697626"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697583"/>
/// </summary>
/// <remarks>两支官方应答表逐字段同形（<c>total_count</c> / <c>count</c> / <c>data[]</c> / <c>offset</c> / <c>links</c>）⇒ 共用同一根类型，见守卫 CAP-B4。</remarks>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalBankPageResponse : WechatPayResponse
{
    /// <summary>【银行列表】 查询到的银行列表数组（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<CapitalBankPageResponseDataItem>? Data { get; set; }

    /// <summary>【请求资源条数】 该次请求实际返回的资源条数（官方字段路径 <c>count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【请求资源起始位置】 该次请求资源的起始位置，请求参数原样返回（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【相关链接】 请求翻页相关链接（官方字段路径 <c>links</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("links")]
    public CapitalBankPageResponseLinks? Links { get; set; }

    /// <summary>【查询总条数】 查询到的银行数据总条数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

/// <summary>查询银行列表响应·【银行列表】 查询到的银行列表数组。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697626"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697583"/></summary>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalBankPageResponseDataItem
{
    /// <summary>【银行别名】 银行的简称别名（官方字段路径 <c>data.bank_alias</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_alias")]
    public string? BankAlias { get; set; }

    /// <summary>【银行别名编码】 银行别名的编码，与银行别名 bank_alias 一一对应（官方字段路径 <c>data.bank_alias_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_alias_code")]
    public string? BankAliasCode { get; set; }

    /// <summary>【开户银行】 开户银行（官方字段路径 <c>data.account_bank</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>【开户银行编号】 银行唯一编码（官方字段路径 <c>data.account_bank_code</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("account_bank_code")]
    public long? AccountBankCode { get; set; }

    /// <summary>【是否需要支行信息】 填写银行账户时是否必须提供支行信息（官方字段路径 <c>data.need_bank_branch</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_bank_branch")]
    public bool? NeedBankBranch { get; set; }
}

/// <summary>查询银行列表响应·【相关链接】 请求翻页相关链接。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697626"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697583"/></summary>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalBankPageResponseLinks
{
    /// <summary>【下一页链接】 使用同样的 limit 进行下一页查询时的请求链接。如果已经到最后时，为空（官方字段路径 <c>links.next</c>，选填，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("next")]
    public string? Next { get; set; }

    /// <summary>【上一页链接】 使用同样的 limit 进行上一页查询时的请求链接。如果是第一页，为空（官方字段路径 <c>links.prev</c>，选填，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("prev")]
    public string? Prev { get; set; }

    /// <summary>【当前链接】 当前的请求链接（官方字段路径 <c>links.self</c>，选填，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("self")]
    public string? Self { get; set; }
}

/// <summary>
/// 查询支行列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697673"/>
/// </summary>
/// <remarks>
/// 官方应答表除翻页骨架（<c>total_count</c> / <c>count</c> / <c>data[]</c> / <c>offset</c> / <c>links</c>）外，
/// <b>根层还另印了四个银行定位键</b>（<c>account_bank</c> / <c>account_bank_code</c> / <c>bank_alias</c> / <c>bank_alias_code</c>）
/// ⇒ 照页建模、不得判为官方笔误删除，也不得与 <see cref="CapitalBankPageResponse"/> 合并（两支 data 元素字段集不同），见守卫 CAP-B3。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalBranchPageResponse : WechatPayResponse
{
    /// <summary>【支行列表】 查询到的支行列表数组（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<CapitalBranchPageResponseDataItem>? Data { get; set; }

    /// <summary>【请求资源条数】 该次请求实际返回的资源条数（官方字段路径 <c>count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【请求资源起始位置】 该次请求资源的起始位置，请求参数原样返回（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【相关链接】 请求翻页相关链接（官方字段路径 <c>links</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("links")]
    public CapitalBranchPageResponseLinks? Links { get; set; }

    /// <summary>【开户银行】 开户银行（官方字段路径 <c>account_bank</c>，<b>必填</b>，类型 <c>string(128)</c>）。官方把该键印在<b>应答根层</b>而非 data 元素内，照页保留。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>【开户银行编号】 银行唯一编码（官方字段路径 <c>account_bank_code</c>，<b>必填</b>，类型 <c>integer</c>）。同 <see cref="AccountBank"/>，属官方页根层形态。</summary>
    [JsonPropertyName("account_bank_code")]
    public long? AccountBankCode { get; set; }

    /// <summary>【银行别名】 银行的简称别名（官方字段路径 <c>bank_alias</c>，<b>必填</b>，类型 <c>string(128)</c>）。同 <see cref="AccountBank"/>，属官方页根层形态。</summary>
    [JsonPropertyName("bank_alias")]
    public string? BankAlias { get; set; }

    /// <summary>【银行别名编码】 银行别名的编码，与银行别名 bank_alias 一一对应（官方字段路径 <c>bank_alias_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。同 <see cref="AccountBank"/>，属官方页根层形态。</summary>
    [JsonPropertyName("bank_alias_code")]
    public string? BankAliasCode { get; set; }

    /// <summary>【查询总条数】 查询到的支行数据总条数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

/// <summary>查询支行列表响应·【支行列表】 查询到的支行列表数组。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697673"/></summary>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalBranchPageResponseDataItem
{
    /// <summary>【银行支行名称】 开户支行名称（官方字段路径 <c>data.bank_branch_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_branch_name")]
    public string? BankBranchName { get; set; }

    /// <summary>【银行支行编号】 开户支行编号（官方字段路径 <c>data.bank_branch_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }
}

/// <summary>查询支行列表响应·【相关链接】 请求翻页相关链接。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697673"/></summary>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalBranchPageResponseLinks
{
    /// <summary>【下一页链接】 使用同样的 limit 进行下一页查询时的请求链接。如果已经到最后时，为空（官方字段路径 <c>links.next</c>，选填，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("next")]
    public string? Next { get; set; }

    /// <summary>【上一页链接】 使用同样的 limit 进行上一页查询时的请求链接。如果是第一页，为空（官方字段路径 <c>links.prev</c>，选填，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("prev")]
    public string? Prev { get; set; }

    /// <summary>【当前链接】 当前的请求链接（官方字段路径 <c>links.self</c>，选填，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("self")]
    public string? Self { get; set; }
}

/// <summary>
/// 获取对私银行卡号开户银行响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697541"/>
/// </summary>
/// <remarks>应答表只有 <c>total_count</c> + <c>data[]</c>，<b>无</b> <c>count</c> / <c>offset</c> / <c>links</c> 翻页骨架 ⇒ 与 <see cref="CapitalBankPageResponse"/> 不同形，不得共用（守卫 CAP-B4）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalBankByAccountResponse : WechatPayResponse
{
    /// <summary>【银行列表】 查询到的开户银行列表数组（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<CapitalBankByAccountResponseDataItem>? Data { get; set; }

    /// <summary>【查询总条数】 查询到的银行数据总条数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

/// <summary>获取对私银行卡号开户银行响应·【银行列表】 查询到的开户银行列表数组。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697541"/></summary>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalBankByAccountResponseDataItem
{
    /// <summary>【银行别名】 银行的简称别名（官方字段路径 <c>data.bank_alias</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_alias")]
    public string? BankAlias { get; set; }

    /// <summary>【银行别名编码】 银行别名的编码，与银行别名 bank_alias 一一对应（官方字段路径 <c>data.bank_alias_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_alias_code")]
    public string? BankAliasCode { get; set; }

    /// <summary>【开户银行】 开户银行（官方字段路径 <c>data.account_bank</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>【开户银行编号】 银行唯一编码（官方字段路径 <c>data.account_bank_code</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("account_bank_code")]
    public long? AccountBankCode { get; set; }

    /// <summary>【是否需要支行信息】 填写银行账户时是否必须提供支行信息（官方字段路径 <c>data.need_bank_branch</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_bank_branch")]
    public bool? NeedBankBranch { get; set; }
}
