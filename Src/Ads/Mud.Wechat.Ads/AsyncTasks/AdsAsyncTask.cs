// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.AsyncTasks;

/// <summary>
/// <c>POST /v3.0/async_tasks/add</c> 的请求体（2026-10-11 L3 核验，4 键）。
/// </summary>
/// <remarks>
/// 必填：<c>account_id</c> / <c>task_name</c> / <c>task_type</c>。<c>task_type</c> 决定
/// <see cref="TaskSpec"/> 里三支 spec 中哪一支生效（官方互斥）；
/// <c>task_type</c> 可选值未逐项核验（与 spec 键名一一对应）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsAsyncTaskAddRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，必填）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>任务名（官方 <c>task_name</c>，必填）。</summary>
    [JsonPropertyName("task_name")]
    public string? TaskName { get; set; }

    /// <summary>任务类型（官方 <c>task_type</c>，必填，<c>string</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("task_type")]
    public string? TaskType { get; set; }

    /// <summary>任务规格（官方 <c>task_spec</c>；三支互斥 spec 见 <see cref="AdsAsyncTaskSpec"/>）。</summary>
    [JsonPropertyName("task_spec")]
    public AdsAsyncTaskSpec? TaskSpec { get; set; }
}

/// <summary>任务规格容器（官方 <c>task_spec</c>，三支互斥 spec）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsAsyncTaskSpec
{
    /// <summary>创建安卓渠道包任务规格（官方 <c>task_type_create_android_channel_package_spec</c>）。</summary>
    [JsonPropertyName("task_type_create_android_channel_package_spec")]
    public AdsTaskCreateAndroidChannelPackageSpec? CreateAndroidChannelPackage { get; set; }

    /// <summary>更新安卓渠道包任务规格（官方 <c>task_type_update_android_channel_package_spec</c>）。</summary>
    [JsonPropertyName("task_type_update_android_channel_package_spec")]
    public AdsTaskUpdateAndroidChannelPackageSpec? UpdateAndroidChannelPackage { get; set; }

    /// <summary>清理创意资产任务规格（官方 <c>task_type_delete_creative_asset_spec</c>）。</summary>
    [JsonPropertyName("task_type_delete_creative_asset_spec")]
    public AdsTaskDeleteCreativeAssetSpec? DeleteCreativeAsset { get; set; }
}

/// <summary>创建安卓渠道包任务规格（官方 <c>task_type_create_android_channel_package_spec</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsTaskCreateAndroidChannelPackageSpec
{
    /// <summary>Myapp 授权 key（官方 <c>myapp_auth_key</c>，必填）。</summary>
    [JsonPropertyName("myapp_auth_key")]
    public string? MyappAuthKey { get; set; }

    /// <summary>安卓应用 id（官方 <c>android_app_id</c>，必填，<c>integer</c>）。</summary>
    [JsonPropertyName("android_app_id")]
    public long? AndroidAppId { get; set; }

    /// <summary>渠道包规格列表（官方 <c>android_channel_package_spec</c>，必填，<c>struct[]</c>）。</summary>
    [JsonPropertyName("android_channel_package_spec")]
    public List<AdsAndroidChannelPackageSpec>? AndroidChannelPackageSpec { get; set; }
}

/// <summary>安卓渠道包规格（创建任务版：官方元素只有 <c>package_name</c> / <c>download_url</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsAndroidChannelPackageSpec
{
    /// <summary>包名（官方 <c>package_name</c>，必填）。</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }

    /// <summary>下载地址（官方 <c>download_url</c>，必填）。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }
}

/// <summary>安卓渠道包规格（更新任务版：官方元素多 <c>channel_package_id</c>，与创建版不同构 ⇒ 分建）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsAndroidChannelPackageUpdateSpec
{
    /// <summary>渠道包 id（官方 <c>channel_package_id</c>，必填；仅更新任务携带）。</summary>
    [JsonPropertyName("channel_package_id")]
    public string? ChannelPackageId { get; set; }

    /// <summary>包名（官方 <c>package_name</c>，必填）。</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }

    /// <summary>下载地址（官方 <c>download_url</c>，必填）。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }
}

/// <summary>更新安卓渠道包任务规格（官方 <c>task_type_update_android_channel_package_spec</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsTaskUpdateAndroidChannelPackageSpec
{
    /// <summary>Myapp 授权 key（官方 <c>myapp_auth_key</c>，必填）。</summary>
    [JsonPropertyName("myapp_auth_key")]
    public string? MyappAuthKey { get; set; }

    /// <summary>安卓应用 id（官方 <c>android_app_id</c>，必填，<c>integer</c>）。</summary>
    [JsonPropertyName("android_app_id")]
    public long? AndroidAppId { get; set; }

    /// <summary>渠道包规格列表（官方 <c>android_channel_package_spec</c>，必填，<c>struct[]</c>；元素带渠道包 id）。</summary>
    [JsonPropertyName("android_channel_package_spec")]
    public List<AdsAndroidChannelPackageUpdateSpec>? AndroidChannelPackageSpec { get; set; }
}

/// <summary>清理创意资产任务规格（官方 <c>task_type_delete_creative_asset_spec</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsTaskDeleteCreativeAssetSpec
{
    /// <summary>组件 id 列表（官方 <c>component_id_list</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("component_id_list")]
    public List<long>? ComponentIdList { get; set; }

    /// <summary>图片 id 列表（官方 <c>image_id_list</c>，<c>string[]</c>）。</summary>
    [JsonPropertyName("image_id_list")]
    public List<string>? ImageIdList { get; set; }

    /// <summary>视频 id 列表（官方 <c>video_id_list</c>，<c>string[]</c>）。</summary>
    [JsonPropertyName("video_id_list")]
    public List<string>? VideoIdList { get; set; }

    /// <summary>清理策略（官方 <c>component_clean_strategy</c>）。</summary>
    [JsonPropertyName("component_clean_strategy")]
    public AdsComponentCleanStrategy? ComponentCleanStrategy { get; set; }
}

/// <summary>组件清理策略（官方 <c>component_clean_strategy</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsComponentCleanStrategy
{
    /// <summary>是否删除创意资产（官方 <c>is_delete_creative_asset</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_delete_creative_asset")]
    public bool? IsDeleteCreativeAsset { get; set; }

    /// <summary>是否解绑组件化创意（官方 <c>is_unbind_dynamic_creatives</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_unbind_dynamic_creatives")]
    public bool? IsUnbindDynamicCreatives { get; set; }

    /// <summary>是否删除不完整的组件化创意（官方 <c>is_delete_incomplete_dynamic_creatives</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_delete_incomplete_dynamic_creatives")]
    public bool? IsDeleteIncompleteDynamicCreatives { get; set; }

    /// <summary>关联清理的创意资产类型列表（官方 <c>delete_related_creative_asset_type_list</c>，<c>array</c>；
    /// 元素类型官方标注 array 未细标，以字符串承载）。</summary>
    [JsonPropertyName("delete_related_creative_asset_type_list")]
    public List<string>? DeleteRelatedCreativeAssetTypeList { get; set; }
}

/// <summary><c>async_tasks/add</c> 的 <c>data</c> 载荷（只有任务 id）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsTaskIdData
{
    /// <summary>任务 id（官方 <c>task_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("task_id")]
    public long? TaskId { get; set; }
}

/// <summary><c>POST /v3.0/async_tasks/add</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsAsyncTaskAddResponse : AdsResponse<AdsTaskIdData>
{
}

/// <summary>异步任务执行结果（官方 <c>result</c>；与 <c>async_reports/get</c> 的 <c>result</c> 外层同构但
/// <c>data</c> 形状完全不同（任务产物列表 vs 报表），D5 按域分建）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsAsyncTaskExecutionResult
{
    /// <summary>任务结果码（官方 <c>code</c>，<c>0</c> = 任务执行成功；可空 —— 任务未完成时缺省）。</summary>
    [JsonPropertyName("code")]
    public int? Code { get; set; }

    /// <summary>任务结果描述（官方 <c>message</c>）。</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>任务产物（官方 <c>data</c>；四种产物列表按任务类型出现）。</summary>
    [JsonPropertyName("data")]
    public AdsAsyncTaskResultPayload? Data { get; set; }
}

/// <summary>异步任务产物（官方 <c>result.data</c>，四种列表）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsAsyncTaskResultPayload
{
    /// <summary>文件信息列表（官方 <c>file_info_list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("file_info_list")]
    public List<AdsTaskFileInfo>? FileInfoList { get; set; }

    /// <summary>渠道包信息列表（官方 <c>channel_package_info_list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("channel_package_info_list")]
    public List<AdsChannelPackageInfo>? ChannelPackageInfoList { get; set; }

    /// <summary>联合渠道包信息列表（官方 <c>union_channel_package_info_list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("union_channel_package_info_list")]
    public List<AdsUnionChannelPackageInfo>? UnionChannelPackageInfoList { get; set; }

    /// <summary>素材预审结果列表（官方 <c>review_element_prereview_result_list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("review_element_prereview_result_list")]
    public List<AdsReviewElementResult>? ReviewElementPrereviewResultList { get; set; }
}

/// <summary>任务产物文件（官方 <c>file_info_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsTaskFileInfo
{
    /// <summary>文件 id（官方 <c>file_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("file_id")]
    public long? FileId { get; set; }

    /// <summary>MD5（官方 <c>md5</c>）。</summary>
    [JsonPropertyName("md5")]
    public string? Md5 { get; set; }
}

/// <summary>渠道包产物（官方 <c>channel_package_info_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsChannelPackageInfo
{
    /// <summary>安卓应用 id（官方 <c>android_app_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("android_app_id")]
    public long? AndroidAppId { get; set; }

    /// <summary>包名（官方 <c>package_name</c>）。</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }

    /// <summary>状态（官方 <c>status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>错误码（官方 <c>error_code</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("error_code")]
    public string? ErrorCode { get; set; }

    /// <summary>创建时间戳（官方 <c>created_time</c>，秒级）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>最后修改时间戳（官方 <c>last_modified_time</c>，秒级）。</summary>
    [JsonPropertyName("last_modified_time")]
    public long? LastModifiedTime { get; set; }

    /// <summary>渠道包 id（官方 <c>channel_package_id</c>）。</summary>
    [JsonPropertyName("channel_package_id")]
    public string? ChannelPackageId { get; set; }
}

/// <summary>联合渠道包产物（官方 <c>union_channel_package_info_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsUnionChannelPackageInfo
{
    /// <summary>联合安卓应用 id（官方 <c>android_union_app_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("android_union_app_id")]
    public long? AndroidUnionAppId { get; set; }

    /// <summary>包名（官方 <c>package_name</c>）。</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }

    /// <summary>状态（官方 <c>status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>创建时间戳（官方 <c>created_time</c>，秒级）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>最后修改时间戳（官方 <c>last_modified_time</c>，秒级）。</summary>
    [JsonPropertyName("last_modified_time")]
    public long? LastModifiedTime { get; set; }
}

/// <summary>素材预审结果（官方 <c>review_element_prereview_result_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsReviewElementResult
{
    /// <summary>元素类型（官方 <c>element_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("element_type")]
    public string? ElementType { get; set; }

    /// <summary>元素内容（官方 <c>element_content</c>）。</summary>
    [JsonPropertyName("element_content")]
    public string? ElementContent { get; set; }

    /// <summary>风险等级（官方 <c>risk_level</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("risk_level")]
    public string? RiskLevel { get; set; }

    /// <summary>预审明细（官方 <c>pre_review_details</c>，<c>struct[]</c>；官方未展开到叶
    /// ⇒ 以开放元素承载，消费需求驱动时再建型）。</summary>
    [JsonPropertyName("pre_review_details")]
    public List<System.Text.Json.JsonElement>? PreReviewDetails { get; set; }
}

/// <summary>异步任务条目（官方 <c>async_tasks/get</c> 的 <c>data.list[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsAsyncTaskInfo
{
    /// <summary>任务 id（官方 <c>task_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("task_id")]
    public long? TaskId { get; set; }

    /// <summary>任务名（官方 <c>task_name</c>）。</summary>
    [JsonPropertyName("task_name")]
    public string? TaskName { get; set; }

    /// <summary>任务类型（官方 <c>task_type</c>，<c>string</c>）。</summary>
    [JsonPropertyName("task_type")]
    public string? TaskType { get; set; }

    /// <summary>任务状态（官方 <c>status</c>，<c>enum</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>创建时间戳（官方 <c>created_time</c>，秒级）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>执行结果（官方 <c>result</c>；任务未完成时缺省）。</summary>
    [JsonPropertyName("result")]
    public AdsAsyncTaskExecutionResult? Result { get; set; }
}

/// <summary><c>async_tasks/get</c> 的 <c>data</c> 载荷。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsAsyncTaskListData
{
    /// <summary>任务列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsAsyncTaskInfo>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary><c>GET /v3.0/async_tasks/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "AsyncTasks")]
public class AdsAsyncTaskGetResponse : AdsResponse<AdsAsyncTaskListData>
{
}
