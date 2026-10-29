// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Service;

/// <summary>
/// 通讯录 id 转译请求体（<c>/cgi-bin/service/contact/id_translate</c>，官方 91846）。
/// </summary>
/// <remarks>
/// <b>异步端点</b>：本接口只回 <c>jobid</c>，结果须经 <c>/cgi-bin/service/batch/getresult</c>（官方 91882）轮询。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class TranslateServiceContactIdsRequest
{
    /// <summary>
    /// 获取或设置授权企业 corpid（<c>auth_corpid</c>，必填）。
    /// </summary>
    [JsonPropertyName("auth_corpid")]
    public string? AuthCorpId { get; set; }

    /// <summary>
    /// 获取或设置需要转译的文件的 media_id 列表（<c>media_id_list</c>，必填，不超过 20 个文件）。
    /// <para>
    /// 官方限制：只支持后缀名为 xls/xlsx、doc/docx、csv、txt 的文件；csv/txt 必须为 UTF-8 编码；
    /// media_id 获取方式参见「服务商上传临时素材」。
    /// </para>
    /// </summary>
    [JsonPropertyName("media_id_list")]
    public IList<string>? MediaIdList { get; set; }

    /// <summary>
    /// 获取或设置转译完打包的文件名（<c>output_file_name</c>，不需带后缀）。
    /// <para>
    /// 官方语义：后台会打包成 zip 并自动拼接 <c>.zip</c> 后缀；<c>media_id_list</c> 中文件个数大于 1 时该字段<b>必填</b>；
    /// 个数等于 1 且未填该字段时，转译完<b>不</b>打包成压缩文件。
    /// </para>
    /// </summary>
    [JsonPropertyName("output_file_name")]
    public string? OutputFileName { get; set; }

    /// <summary>
    /// 获取或设置输出文件格式（<c>output_file_format</c>）。
    /// <para>
    /// 官方语义：若不指定则输出格式与输入格式相同；当前<b>仅支持</b>输出为 pdf 格式。
    /// 若 <c>media_id_list</c> 中存在相同前缀名的文件，输出命名规则为「文件前缀名_文件格式后缀.pdf」。
    /// </para>
    /// </summary>
    [JsonPropertyName("output_file_format")]
    public string? OutputFileFormat { get; set; }
}

/// <summary>
/// 通讯录 id 转译响应体（官方 91846，仅回异步任务 id）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class TranslateServiceContactIdsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置异步任务 id（<c>jobid</c>，最大 64 字节；用于「获取异步任务结果」接口传递）。
    /// </summary>
    [JsonPropertyName("jobid")]
    public string? JobId { get; set; }
}

/// <summary>
/// 通讯录成员排序请求体（<c>/cgi-bin/service/contact/sort</c>，官方 92093）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class SortServiceContactsRequest
{
    /// <summary>
    /// 获取或设置查询的企业 corpid（<c>auth_corpid</c>，必填）。
    /// </summary>
    [JsonPropertyName("auth_corpid")]
    public string? AuthCorpId { get; set; }

    /// <summary>
    /// 获取或设置要排序的 userid 列表（<c>useridlist</c>，必填，最多 1000 个）。
    /// </summary>
    [JsonPropertyName("useridlist")]
    public IList<string>? UserIdList { get; set; }

    /// <summary>
    /// 获取或设置排序选项列表（<c>sort_options</c>）。
    /// <para>官方语义：指定多个时按<b>优先级</b>排序（先按第一项，相同再按第二项，依此类推）。</para>
    /// </summary>
    [JsonPropertyName("sort_options")]
    public IList<ServiceContactSortOption>? SortOptions { get; set; }
}

/// <summary>
/// 排序选项（<c>sort_options</c> 元素，官方 92093）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class ServiceContactSortOption
{
    /// <summary>
    /// 获取或设置排序属性（<c>sort_field</c>）：0 根据姓名拼音、1 根据别名。
    /// </summary>
    [JsonPropertyName("sort_field")]
    public int? SortField { get; set; }

    /// <summary>
    /// 获取或设置排序方式（<c>sort_type</c>）：0 升序、1 降序。
    /// </summary>
    [JsonPropertyName("sort_type")]
    public int? SortType { get; set; }
}

/// <summary>
/// 通讯录成员排序响应体（官方 92093）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class SortServiceContactsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置排序后的 userid 列表（<c>useridlist</c>）。
    /// </summary>
    [JsonPropertyName("useridlist")]
    public IList<string>? UserIdList { get; set; }
}
