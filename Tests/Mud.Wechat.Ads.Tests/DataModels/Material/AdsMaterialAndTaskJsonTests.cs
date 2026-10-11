// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.AsyncTasks;
using Mud.Wechat.Ads.DataModels.Common;
using Mud.Wechat.Ads.DataModels.Components;
using Mud.Wechat.Ads.DataModels.Images;
using Mud.Wechat.Ads.DataModels.Videos;

namespace Mud.Wechat.Ads.Tests.DataModels.Material;

/// <summary>
/// 素材（images/videos）、组件（components）与异步任务（async_tasks）四域 DTO 的官方键绑定抽查
/// （2026-10-11 L3 核验后的样本回放；解析器固定源生成上下文合并，AOT 口径一致）。
/// </summary>
public class AdsMaterialAndTaskJsonTests
{
    private static System.Text.Json.JsonSerializerOptions Options()
        => new()
        {
            TypeInfoResolver = System.Text.Json.Serialization.Metadata.JsonTypeInfoResolver.Combine(
                ComponentsJsonContext.Default,
                ImagesJsonContext.Default,
                VideosJsonContext.Default,
                AsyncTasksJsonContext.Default,
                CommonJsonContext.Default),
        };

    private static T? Deserialize<T>(string json)
        where T : class
        => (T?)System.Text.Json.JsonSerializer.Deserialize(json, Options().GetTypeInfo(typeof(T))!);

    [Fact]
    public void ComponentGetResponse_ShouldBindSingularComponentValue_WithOfficialKeys()
    {
        const string json = """
{
  "code": 0,
  "message": "",
  "message_cn": "",
  "data": {
    "list": [
      {
        "account_id": 111,
        "organization_id": 222,
        "component_id": 333,
        "component_value": {
          "title": { "component_id": 1, "value": { "content": "标题" }, "is_deleted": false },
          "jump_info": { "component_id": 2, "value": { "page_type": "PAGE_TYPE_DEFAULT" } }
        },
        "component_sub_type": "COMPONENT_SUB_TYPE_TITLE",
        "is_deleted": false,
        "scene": "SCENE_TYPE_CUSTOM"
      }
    ],
    "page_info": { "page": 1, "page_size": 20, "total_number": 1, "total_page": 1 }
  }
}
""";

        var response = Deserialize<AdsComponentGetResponse>(json);

        response!.IsSuccess.Should().BeTrue();
        var component = response.Data!.List!.Single();
        component.ComponentId.Should().Be(333);
        // component_value 是「单数」形态：40 组件键直接挂 struct（与 creative_components 的数组形态不同构）。
        component.ComponentValue!.Title!.Value!["content"].GetString().Should().Be("标题");
        component.ComponentValue.JumpInfo!.Value!["page_type"].GetString().Should().Be("PAGE_TYPE_DEFAULT");
        component.ComponentValue.Image.Should().BeNull("官方样例未出现的组件键保持 null（不物化）");
    }

    [Fact]
    public void ComponentDeleteRequest_ShouldCarryOfficialEnumValues()
    {
        var request = new AdsComponentDeleteRequest
        {
            AccountId = 111,
            ComponentId = 333,
            DeleteStrategy = "DELETE_STRATEGY_RESTRICTED",
        };

        var json = System.Text.Json.JsonSerializer.Serialize(
            request, ComponentsJsonContext.Default.AdsComponentDeleteRequest);

        json.Should().Contain("\"delete_strategy\":\"DELETE_STRATEGY_RESTRICTED\"");
        json.Should().NotContain("organization_id", "未赋值可空字段不上送");
    }

    [Fact]
    public void ImageGetResponse_ShouldBindOfficialSample()
    {
        const string json = """
{
  "code": 0,
  "message": "",
  "message_cn": "",
  "data": {
    "list": [
      {
        "image_id": "img-1",
        "width": 1280,
        "height": 720,
        "file_size": 10240,
        "type": "TYPE_JPG",
        "signature": "sig",
        "description": "d",
        "preview_url": "https://e.qq.com/p.png",
        "image_usage": "IMAGE_USAGE_TYPE_AD",
        "created_time": 1700000000,
        "status": "STATUS_OK",
        "aigc_flag": "AIGC_FLAG_NO"
      }
    ],
    "page_info": { "page": 1, "page_size": 20, "total_number": 1, "total_page": 1 }
  }
}
""";

        var response = Deserialize<AdsImageGetResponse>(json);

        response!.IsSuccess.Should().BeTrue();
        var image = response.Data!.List!.Single();
        image.ImageId.Should().Be("img-1");
        image.Width.Should().Be(1280);
        image.ImageUsage.Should().Be("IMAGE_USAGE_TYPE_AD");
        image.AigcFlag.Should().Be("AIGC_FLAG_NO");
    }

    [Fact]
    public void ImageUploadResponse_ShouldBindMultipartReply()
    {
        var response = Deserialize<AdsImageUploadResponse>(
            """{"code":0,"message":"","message_cn":"","data":{"image_id":"img-9","image_width":100,"image_height":80,"image_file_size":64,"image_type":"TYPE_PNG","image_signature":"s2","outer_image_id":"o1","preview_url":"https://e.qq.com/p2.png","description":"d2"}}""");

        response!.IsSuccess.Should().BeTrue();
        response.Data!.ImageId.Should().Be("img-9");
        response.Data.ImageWidth.Should().Be(100);
        response.Data.OuterImageId.Should().Be("o1");
    }

    [Fact]
    public void VideoGetResponse_ShouldBindCodecFields_AndUploadReply()
    {
        const string json = """
{
  "code": 0,
  "message": "",
  "message_cn": "",
  "data": {
    "list": [
      {
        "video_id": 444,
        "width": 1920,
        "height": 1080,
        "video_frames": 300,
        "video_fps": 30.0,
        "video_codec": "H264",
        "video_bit_rate": 2000,
        "audio_codec": "AAC",
        "audio_bit_rate": 128,
        "file_size": 1048576,
        "type": "TYPE_MP4",
        "status": "STATUS_OK",
        "cover_id": "cover-1"
      }
    ],
    "page_info": { "page": 1, "page_size": 20, "total_number": 1, "total_page": 1 }
  }
}
""";

        var response = Deserialize<AdsVideoGetResponse>(json);

        response!.IsSuccess.Should().BeTrue();
        var video = response.Data!.List!.Single();
        video.VideoId.Should().Be(444);
        video.VideoFps.Should().Be(30.0m);
        video.VideoCodec.Should().Be("H264");
        video.AudioCodec.Should().Be("AAC");
        video.CoverId.Should().Be("cover-1");

        var upload = Deserialize<AdsVideoUploadResponse>(
            """{"code":0,"message":"","message_cn":"","data":{"video_id":444,"cover_image_id":555}}""");
        upload!.Data!.VideoId.Should().Be(444);
        upload.Data.CoverImageId.Should().Be(555);
    }

    [Fact]
    public void AsyncTaskAddRequest_ShouldSerializeUpdateSpecWithChannelPackageId()
    {
        var request = new AdsAsyncTaskAddRequest
        {
            AccountId = 111,
            TaskName = "t-1",
            TaskType = "TASK_TYPE_UPDATE_ANDROID_CHANNEL_PACKAGE",
            TaskSpec = new AdsAsyncTaskSpec
            {
                UpdateAndroidChannelPackage = new AdsTaskUpdateAndroidChannelPackageSpec
                {
                    MyappAuthKey = "auth",
                    AndroidAppId = 555,
                    AndroidChannelPackageSpec = new List<AdsAndroidChannelPackageUpdateSpec>
                    {
                        new() { ChannelPackageId = "cp-1", PackageName = "com.a", DownloadUrl = "https://e.qq.com/a.apk" },
                    },
                },
            },
        };

        var json = System.Text.Json.JsonSerializer.Serialize(
            request, AsyncTasksJsonContext.Default.AdsAsyncTaskAddRequest);

        json.Should().Contain("\"task_type_update_android_channel_package_spec\":")
            .And.Contain("\"channel_package_id\":\"cp-1\"")
            .And.NotContain("task_type_create_android_channel_package_spec", "未赋值的互斥 spec 不上送");
    }

    [Fact]
    public void AsyncTaskGetResponse_ShouldBindTwoLayerResult()
    {
        const string json = """
{
  "code": 0,
  "message": "",
  "message_cn": "",
  "data": {
    "list": [
      {
        "task_id": 666,
        "task_name": "t-2",
        "task_type": "TASK_TYPE_CREATE_ANDROID_CHANNEL_PACKAGE",
        "status": "TASK_STATUS_FINISHED",
        "created_time": 1700000000,
        "result": {
          "code": 0,
          "message": "ok",
          "data": {
            "channel_package_info_list": [
              {
                "android_app_id": 555,
                "package_name": "com.a",
                "status": "CHANNEL_PACKAGE_STATUS_SUCCESS",
                "error_code": "NONE",
                "channel_package_id": "cp-2"
              }
            ]
          }
        }
      }
    ],
    "page_info": { "page": 1, "page_size": 20, "total_number": 1, "total_page": 1 }
  }
}
""";

        var response = Deserialize<AdsAsyncTaskGetResponse>(json);

        response!.IsSuccess.Should().BeTrue("外层信封 code 只表示「查询被受理」");
        var task = response.Data!.List!.Single();
        task.TaskId.Should().Be(666);
        task.Status.Should().Be("TASK_STATUS_FINISHED");
        // 双层判定：任务执行结果看 result.code（result 层无 message_cn）。
        task.Result!.Code.Should().Be(0);
        task.Result.Data!.ChannelPackageInfoList!.Single().ChannelPackageId.Should().Be("cp-2");
    }
}
