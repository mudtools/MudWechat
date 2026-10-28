// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>数据库记录管理响应。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbrecordmanage.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbManageDatabaseRecordResponse : OpenPlatformResponse
{
    /// <summary>插入成功的数据集合主键_id。insert时返回</summary>
    [JsonPropertyName("id_list")]
    public List<string>? IdList { get; set; }

    /// <summary>删除记录数量。delete时返回</summary>
    [JsonPropertyName("deleted")]
    public long? Deleted { get; set; }

    /// <summary>更新条件匹配到的结果数。update时返回</summary>
    [JsonPropertyName("matched")]
    public long? Matched { get; set; }

    /// <summary>修改的记录数，update时返回。注意：使用set操作新插入的数据不计入修改数目</summary>
    [JsonPropertyName("modified")]
    public long? Modified { get; set; }

    /// <summary>新插入记录的id，update时返回。注意：只有使用set操作新插入数据时这个字段会有值</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>分页信息。query时返回</summary>
    [JsonPropertyName("pager")]
    public OpenPlatformComponentTcbManageDatabaseRecordResponsePager? Pager { get; set; }

    /// <summary>记录数组。query时返回</summary>
    [JsonPropertyName("data")]
    public List<JsonElement>? Data { get; set; }
}
