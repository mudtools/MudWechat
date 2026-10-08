// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Wedrive;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 微盘模块（Wedrive 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定。
/// <para>
/// 六功能族形态：
/// 管理空间族（新建空间 + 重命名空间 + 解散空间 + 获取空间信息 4 端点）；
/// 管理空间权限族（添加成员/部门 + 移除成员/部门 + 安全设置 + 获取邀请链接 +
/// 获取空间信息（新版）5 端点）；
/// 管理文件族（获取文件列表 + 上传文件 + 文件分块上传（一页三路由：初始化/分块/完成） +
/// 下载文件 + 新建文件夹/文档 + 重命名文件 + 移动文件 + 删除文件 + 获取文件信息，9 页文档承载 11 条路由）；
/// 管理文件权限族（新增成员 + 删除成员 + 分享设置 + 获取分享链接 + 获取文件权限信息 + 修改文件安全设置 6 端点）；
/// 版本和容量管理族（获取盘专业版信息 + 获取盘容量信息 2 端点，官方单文档页两路由、空请求体）；
/// 高级功能账号管理族（分配/取消/列表 3 端点，官方仅自建应用开放——代开发与第三方标注「暂不支持」，
/// 零端点父接口 + 仅自建子接口承载，路由挂 /cgi-bin/wedrive/vip/ 段）。
/// 前五族均为三类应用公共面收敛父接口 + 三个应用类型空标记子接口。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：微盘域 31 条路由官方全部即 POST（含仅查询语义的
/// space_info / new_space_info / space_share / file_list / file_info / file_share / get_file_permission /
/// mng_pro_info / mng_capacity / vip/list）；「获取空间信息」存在新旧两条路由——旧版
/// <c>space_info</c> 挂官方「管理空间」分组，新版 <c>new_space_info</c> 额外返回安全设置
/// secure_setting 与已退出空间成员 quit_userid、官方归入「管理空间权限」分组，两版响应结构
/// 不同构，本 SDK 以两个类型分别承载；「禁止文件分享到企业外」写入口（space_setting）官方
/// 字段名作 <c>ban_share_external</c>、读取回显（new_space_info.secure_setting）作
/// <c>enable_share_external</c>，两侧不同名；移除成员/部门（space_acl_del）官方请求形态不携带
/// auth 字段；空间 ID 官方字段名作 <c>spaceid</c>（无下划线）；应用空间管理员（auth = 7）
/// 最多可指定 3 个且不支持设置部门；安全设置仅支持设置由本应用创建的空间。
/// 管理文件族：文件列表分页用 start + limit（start 首次填 0、后续填上一次返回的 next_start），
/// 区别于本仓多数域的 cursor + limit；file_list 响应官方为对象内含 item 数组（<c>file_list.item</c>）；
/// 移动/删除文件的请求字段官方即作 <c>fileid</c> 但为数组形态；上传/下载文件官方以
/// spaceid/fatherid 与 selected_ticket（或 fileid 与 selected_ticket）「必须填且仅填其中一组」的
/// 互斥参数组表达；文件名统一最多 255 个字符（英文算 1 个，汉字算 2 个）。
/// 管理文件权限族：获取文件权限信息路由不走 <c>wedrive/file_</c> 前缀（官方即 get_file_permission），
/// 其响应 file_member_list 官方参数表列 obj、返回示例实为数组；文件权限成员的
/// type/departmentid 官方标注「后续将废弃」、auth 取值仅 1（仅浏览/仅下载）；
/// 修改文件安全设置仅支持在线文档类型，watermark 写入口仅开放 4 个字段
/// （force_by_admin / force_by_space_admin 为读取回显）。
/// 版本和容量管理族：官方单文档页承载 2 条路由（mng_pro_info / mng_capacity），
/// 请求包体均为空对象 <c>{}</c>，本 SDK 不声明请求 DTO（对齐 get_openid_migration 无请求体先例）。
/// 高级功能账号管理族：路由挂 <c>/cgi-bin/wedrive/vip/</c> 段；账号列表用 cursor + limit 分页
/// （has_more / next_cursor），官方明确「不保证每次返回的数据刚好为指定 limit，
/// 必须用返回的 has_more 判断是否继续请求」；分配/取消单次操作 userid 上限 100 个。
/// </para>
/// </remarks>
public class WechatWedriveContractGuards
{
    private const string WedriveRegistryGroupName = "Wedrive";

    // ------------------------------------------------------------------
    // 路由表：9 条官方路由，全部 POST（勿「顺手统一」为 GET）。
    // ------------------------------------------------------------------

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        WedriveSpaceRoutes =
        {
            // 新建空间（自建 93655 / 第三方 95857 / 代开发 96845；三类公共收敛父接口；官方即 POST）。
            (typeof(IWechatWorkWedriveSpaceService),
                nameof(IWechatWorkWedriveSpaceService.CreateSpaceAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_create"),
            // 重命名空间（自建 97856 / 第三方 97872 / 代开发 97862；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveSpaceService),
                nameof(IWechatWorkWedriveSpaceService.RenameSpaceAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_rename"),
            // 解散空间（自建 97857 / 第三方 97873 / 代开发 97863；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveSpaceService),
                nameof(IWechatWorkWedriveSpaceService.DismissSpaceAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_dismiss"),
            // 获取空间信息·旧版（自建 97858 / 第三方 97874 / 代开发 97864；三类公共收敛父接口；
            // 挂官方「管理空间」分组）。
            (typeof(IWechatWorkWedriveSpaceService),
                nameof(IWechatWorkWedriveSpaceService.GetSpaceInfoAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_info"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        WedriveSpaceAclRoutes =
        {
            // 添加成员/部门（自建 93656 / 第三方 95858 / 代开发 96846；三类公共收敛父接口；
            // auth = 7 应用空间管理员最多 3 个且不支持部门）。
            (typeof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkWedriveSpaceAclService.AddSpaceAclAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_acl_add"),
            // 移除成员/部门（自建 97875 / 第三方 97947 / 代开发 97910；三类公共收敛父接口；
            // 官方请求形态不携带 auth 字段）。
            (typeof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkWedriveSpaceAclService.DelSpaceAclAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_acl_del"),
            // 安全设置（自建 97876 / 第三方 97948 / 代开发 97911；三类公共收敛父接口；
            // 仅支持设置由本应用创建的空间）。
            (typeof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkWedriveSpaceAclService.SetSpaceSettingAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_setting"),
            // 获取邀请链接（自建 97877 / 第三方 97949 / 代开发 97912；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkWedriveSpaceAclService.GetSpaceShareUrlAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_share"),
            // 获取空间信息·新版（自建 97878 / 第三方 97950 / 代开发 97913；三类公共收敛父接口；
            // 官方将其归入「管理空间权限」分组而非「管理空间」）。
            (typeof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkWedriveSpaceAclService.GetNewSpaceInfoAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/new_space_info"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        WedriveFileRoutes =
        {
            // 获取文件列表（自建 93657 / 第三方 95859 / 代开发 96847；三类公共收敛父接口；
            // start + limit 分页，limit 不超过 1000）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.GetFileListAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_list"),
            // 上传文件（自建 97880 / 第三方 97951 / 代开发 97914；三类公共收敛父接口；
            // 整文件 base64 上限 10M）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.UploadFileAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_upload"),
            // 分块上传初始化（自建 98004 / 第三方 98005 / 代开发 98007；三类公共收敛父接口；
            // 文件分块上传官方单文档页承载 3 条路由；size 最大 20G，命中秒传直接返回 fileid）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.InitFileUploadAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_upload_init"),
            // 分块上传文件（自建 98004 / 第三方 98005 / 代开发 98007；三类公共收敛父接口；
            // 2M 固定分块，index 从 1 开始，官方建议并发数不超过 10）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.UploadFilePartAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_upload_part"),
            // 分块上传完成（自建 98004 / 第三方 98005 / 代开发 98007；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.FinishFileUploadAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_upload_finish"),
            // 下载文件（自建 97881 / 第三方 97953 / 代开发 97915；三类公共收敛父接口；
            // download_url 有效期 2 小时，仅支持普通文件）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.DownloadFileAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_download"),
            // 新建文件夹/文档（自建 97882 / 第三方 97954 / 代开发 97916；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.CreateFileAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_create"),
            // 重命名文件（自建 97883 / 第三方 97955 / 代开发 97917；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.RenameFileAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_rename"),
            // 移动文件（自建 97884 / 第三方 97956 / 代开发 97918；三类公共收敛父接口；
            // fileid 为数组形态、支持批量）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.MoveFileAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_move"),
            // 删除文件（自建 97885 / 第三方 97957 / 代开发 97919；三类公共收敛父接口；
            // fileid 为数组形态、支持批量）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.DeleteFileAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_delete"),
            // 获取文件信息（自建 97886 / 第三方 97958 / 代开发 97920；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkWedriveFileService.GetFileInfoAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_info"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        WedriveFileAclRoutes =
        {
            // 新增成员（自建 93658 / 第三方 95860 / 代开发 96848；三类公共收敛父接口；
            // 文件权限侧 auth 取值仅 1，type/departmentid 官方标注后续将废弃）。
            (typeof(IWechatWorkWedriveFileAclService),
                nameof(IWechatWorkWedriveFileAclService.AddFileAclAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_acl_add"),
            // 删除成员（自建 97888 / 第三方 97959 / 代开发 97922；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveFileAclService),
                nameof(IWechatWorkWedriveFileAclService.DelFileAclAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_acl_del"),
            // 分享设置（自建 97889 / 第三方 97960 / 代开发 97923；三类公共收敛父接口；
            // auth_scope 取值 1~5，4/5 仅有管理员时可设置）。
            (typeof(IWechatWorkWedriveFileAclService),
                nameof(IWechatWorkWedriveFileAclService.SetFileSettingAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_setting"),
            // 获取分享链接（自建 97890 / 第三方 97961 / 代开发 97924；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveFileAclService),
                nameof(IWechatWorkWedriveFileAclService.GetFileShareUrlAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_share"),
            // 获取文件权限信息（自建 97891 / 第三方 97962 / 代开发 97925；三类公共收敛父接口；
            // 路由不走 wedrive/file_ 前缀，官方即 get_file_permission）。
            (typeof(IWechatWorkWedriveFileAclService),
                nameof(IWechatWorkWedriveFileAclService.GetFilePermissionAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/get_file_permission"),
            // 修改文件安全设置（自建 97892 / 第三方 97965 / 代开发 97926；三类公共收敛父接口；
            // 仅支持在线文档类型）。
            (typeof(IWechatWorkWedriveFileAclService),
                nameof(IWechatWorkWedriveFileAclService.SetFileSecureSettingAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/file_secure_setting"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        WedriveCapacityRoutes =
        {
            // 获取盘专业版信息（自建 95856 / 第三方 95861 / 代开发 96849；三类公共收敛父接口；
            // 官方单文档页承载 mng_pro_info 与 mng_capacity 两条路由，请求包体均为空对象 {}）。
            (typeof(IWechatWorkWedriveCapacityService),
                nameof(IWechatWorkWedriveCapacityService.GetProInfoAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/mng_pro_info"),
            // 获取盘容量信息（自建 95856 / 第三方 95861 / 代开发 96849；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveCapacityService),
                nameof(IWechatWorkWedriveCapacityService.GetCapacityInfoAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/mng_capacity"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        WedriveVipRoutes =
        {
            // 分配高级功能账号（自建 99512；官方仅自建应用开放，代开发/第三方标注「暂不支持」）。
            (typeof(IWechatWorkInternalWedriveVipService),
                nameof(IWechatWorkInternalWedriveVipService.BatchAddVipAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/vip/batch_add"),
            // 取消高级功能账号（自建 99513；官方仅自建应用开放）。
            (typeof(IWechatWorkInternalWedriveVipService),
                nameof(IWechatWorkInternalWedriveVipService.BatchDelVipAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/vip/batch_del"),
            // 获取高级功能账号列表（自建 99514；官方仅自建应用开放；cursor + limit 分页）。
            (typeof(IWechatWorkInternalWedriveVipService),
                nameof(IWechatWorkInternalWedriveVipService.ListVipAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/vip/list"),
        };

    // ------------------------------------------------------------------
    // WR1：全部端点路由与官方契约一致（31 条路由，六族 4 + 5 + 11 + 6 + 2 + 3）。
    // ------------------------------------------------------------------

    [Fact]
    public void WedriveEndpoints_ShouldMatchOfficialRoutes()
    {
        // 管理空间族公共面：4 端点收敛父接口。
        WedriveSpaceRoutes.Should().HaveCount(4, "管理空间族公共面 = 新建 + 重命名 + 解散 + 获取空间信息");
        WedriveSpaceRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/wedrive/", StringComparison.Ordinal), "微盘域路由位于 /cgi-bin/wedrive/ 段");

        // 管理空间权限族公共面：5 端点收敛父接口（新版获取空间信息官方挂本组）。
        WedriveSpaceAclRoutes.Should().HaveCount(5, "管理空间权限族公共面 = 添加/移除成员部门 + 安全设置 + 获取邀请链接 + 新版获取空间信息");
        WedriveSpaceAclRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/wedrive/", StringComparison.Ordinal), "微盘域路由位于 /cgi-bin/wedrive/ 段");

        // 管理文件族公共面：11 端点收敛父接口（分块上传一页三路由）。
        WedriveFileRoutes.Should().HaveCount(11, "管理文件族公共面 = 列表 + 上传 + 分块上传三路由 + 下载 + 新建 + 重命名 + 移动 + 删除 + 获取文件信息");
        WedriveFileRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/wedrive/", StringComparison.Ordinal), "微盘域路由位于 /cgi-bin/wedrive/ 段");

        // 管理文件权限族公共面：6 端点收敛父接口。
        WedriveFileAclRoutes.Should().HaveCount(6, "管理文件权限族公共面 = 新增/删除成员 + 分享设置 + 获取分享链接 + 获取文件权限信息 + 修改文件安全设置");
        WedriveFileAclRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/wedrive/", StringComparison.Ordinal), "微盘域路由位于 /cgi-bin/wedrive/ 段");

        // 版本和容量管理族公共面：2 端点收敛父接口（官方单文档页两路由，空请求体）。
        WedriveCapacityRoutes.Should().HaveCount(2, "版本和容量管理族公共面 = 获取盘专业版信息 + 获取盘容量信息");
        WedriveCapacityRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/wedrive/", StringComparison.Ordinal), "微盘域路由位于 /cgi-bin/wedrive/ 段");

        // 高级功能账号管理族：官方仅自建开放，路由挂 /cgi-bin/wedrive/vip/ 段。
        WedriveVipRoutes.Should().HaveCount(3, "高级功能账号管理族 = 分配 + 取消 + 列表（官方仅自建应用开放）");
        WedriveVipRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/wedrive/vip/", StringComparison.Ordinal), "高级功能账号管理路由挂 /cgi-bin/wedrive/vip/ 段");

        AssertRoutes(WedriveSpaceRoutes);
        AssertRoutes(WedriveSpaceAclRoutes);
        AssertRoutes(WedriveFileRoutes);
        AssertRoutes(WedriveFileAclRoutes);
        AssertRoutes(WedriveCapacityRoutes);
        AssertRoutes(WedriveVipRoutes);

        // 全部官方路由清单锁定（31 条，space_info 与 new_space_info 为两条独立官方路由）。
        var allRoutes = WedriveSpaceRoutes.Concat(WedriveSpaceAclRoutes).Concat(WedriveFileRoutes)
            .Concat(WedriveFileAclRoutes).Concat(WedriveCapacityRoutes).Concat(WedriveVipRoutes)
            .Select(r => r.Route).Distinct().ToList();
        allRoutes.Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/wedrive/space_create",
            "/cgi-bin/wedrive/space_rename",
            "/cgi-bin/wedrive/space_dismiss",
            "/cgi-bin/wedrive/space_info",
            "/cgi-bin/wedrive/space_acl_add",
            "/cgi-bin/wedrive/space_acl_del",
            "/cgi-bin/wedrive/space_setting",
            "/cgi-bin/wedrive/space_share",
            "/cgi-bin/wedrive/new_space_info",
            "/cgi-bin/wedrive/file_list",
            "/cgi-bin/wedrive/file_upload",
            "/cgi-bin/wedrive/file_upload_init",
            "/cgi-bin/wedrive/file_upload_part",
            "/cgi-bin/wedrive/file_upload_finish",
            "/cgi-bin/wedrive/file_download",
            "/cgi-bin/wedrive/file_create",
            "/cgi-bin/wedrive/file_rename",
            "/cgi-bin/wedrive/file_move",
            "/cgi-bin/wedrive/file_delete",
            "/cgi-bin/wedrive/file_info",
            "/cgi-bin/wedrive/file_acl_add",
            "/cgi-bin/wedrive/file_acl_del",
            "/cgi-bin/wedrive/file_setting",
            "/cgi-bin/wedrive/file_share",
            "/cgi-bin/wedrive/get_file_permission",
            "/cgi-bin/wedrive/file_secure_setting",
            "/cgi-bin/wedrive/mng_pro_info",
            "/cgi-bin/wedrive/mng_capacity",
            "/cgi-bin/wedrive/vip/batch_add",
            "/cgi-bin/wedrive/vip/batch_del",
            "/cgi-bin/wedrive/vip/list",
        }, "微盘域全部官方路由须与官方文档一一对应");
        allRoutes.Should().HaveCount(31, "微盘域共 31 条官方路由（新旧获取空间信息为两条独立路由，分块上传与版本容量各为一页多路由）");

        // 无业务负载端点：响应直接用 WechatWorkResponse，不得新建空响应 DTO
        //（重命名 / 解散 / 添加成员部门 / 移除成员部门 / 安全设置 / 分块上传文件 / 删除文件 /
        // 文件权限新增成员 / 文件权限删除成员 / 文件分享设置 / 修改文件安全设置共 11 个）。
        var noPayloadEndpoints = new (Type Interface, string Method)[]
        {
            (typeof(IWechatWorkWedriveSpaceService), nameof(IWechatWorkWedriveSpaceService.RenameSpaceAsync)),
            (typeof(IWechatWorkWedriveSpaceService), nameof(IWechatWorkWedriveSpaceService.DismissSpaceAsync)),
            (typeof(IWechatWorkWedriveSpaceAclService), nameof(IWechatWorkWedriveSpaceAclService.AddSpaceAclAsync)),
            (typeof(IWechatWorkWedriveSpaceAclService), nameof(IWechatWorkWedriveSpaceAclService.DelSpaceAclAsync)),
            (typeof(IWechatWorkWedriveSpaceAclService), nameof(IWechatWorkWedriveSpaceAclService.SetSpaceSettingAsync)),
            (typeof(IWechatWorkWedriveFileService), nameof(IWechatWorkWedriveFileService.UploadFilePartAsync)),
            (typeof(IWechatWorkWedriveFileService), nameof(IWechatWorkWedriveFileService.DeleteFileAsync)),
            (typeof(IWechatWorkWedriveFileAclService), nameof(IWechatWorkWedriveFileAclService.AddFileAclAsync)),
            (typeof(IWechatWorkWedriveFileAclService), nameof(IWechatWorkWedriveFileAclService.DelFileAclAsync)),
            (typeof(IWechatWorkWedriveFileAclService), nameof(IWechatWorkWedriveFileAclService.SetFileSettingAsync)),
            (typeof(IWechatWorkWedriveFileAclService), nameof(IWechatWorkWedriveFileAclService.SetFileSecureSettingAsync)),
        };
        foreach (var (iface, method) in noPayloadEndpoints)
        {
            iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
                .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                    $"{iface.Name}.{method} 仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        }

        // 版本和容量管理族：官方请求包体为空对象 {}，方法不得声明请求 DTO（仅 CancellationToken 一参）。
        foreach (var (iface, method, _, _) in WedriveCapacityRoutes)
        {
            var parameters = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
                .GetParameters();
            parameters.Should().HaveCount(1,
                $"{iface.Name}.{method} 官方请求包体为空对象，不得声明 [Body] 请求参数");
            parameters[0].ParameterType.Should().Be(typeof(CancellationToken),
                $"{iface.Name}.{method} 唯一参数应为 CancellationToken");
        }
    }

    // ------------------------------------------------------------------
    // WR2：接口层级与生成器注册形态（两族继承链、父/子端点数与开放面收敛）。
    // ------------------------------------------------------------------

    [Fact]
    public void WedriveInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        // 管理空间族：公共父 4 端点，三个应用类型子接口均为零端点空标记（9 端点官方对三类应用开放一致）。
        AssertFamily(
            parent: typeof(IWechatWorkWedriveSpaceService),
            parentImplementation: "WechatWorkWedriveSpaceService",
            parentDeclaredEndpointCount: 4,
            new[]
            {
                (typeof(IWechatWorkInternalWedriveSpaceService), 0),
                (typeof(IWechatWorkProviderWedriveSpaceService), 0),
                (typeof(IWechatWorkThirdPartyWedriveSpaceService), 0),
            });

        // 管理空间权限族：公共父 5 端点，三个应用类型子接口均为零端点空标记。
        AssertFamily(
            parent: typeof(IWechatWorkWedriveSpaceAclService),
            parentImplementation: "WechatWorkWedriveSpaceAclService",
            parentDeclaredEndpointCount: 5,
            new[]
            {
                (typeof(IWechatWorkInternalWedriveSpaceAclService), 0),
                (typeof(IWechatWorkProviderWedriveSpaceAclService), 0),
                (typeof(IWechatWorkThirdPartyWedriveSpaceAclService), 0),
            });

        // 管理文件族：公共父 11 端点（分块上传一页三路由），三个应用类型子接口均为零端点空标记。
        AssertFamily(
            parent: typeof(IWechatWorkWedriveFileService),
            parentImplementation: "WechatWorkWedriveFileService",
            parentDeclaredEndpointCount: 11,
            new[]
            {
                (typeof(IWechatWorkInternalWedriveFileService), 0),
                (typeof(IWechatWorkProviderWedriveFileService), 0),
                (typeof(IWechatWorkThirdPartyWedriveFileService), 0),
            });

        // 管理文件权限族：公共父 6 端点，三个应用类型子接口均为零端点空标记。
        AssertFamily(
            parent: typeof(IWechatWorkWedriveFileAclService),
            parentImplementation: "WechatWorkWedriveFileAclService",
            parentDeclaredEndpointCount: 6,
            new[]
            {
                (typeof(IWechatWorkInternalWedriveFileAclService), 0),
                (typeof(IWechatWorkProviderWedriveFileAclService), 0),
                (typeof(IWechatWorkThirdPartyWedriveFileAclService), 0),
            });

        // 版本和容量管理族：公共父 2 端点（官方单文档页两路由），三个应用类型子接口均为零端点空标记。
        AssertFamily(
            parent: typeof(IWechatWorkWedriveCapacityService),
            parentImplementation: "WechatWorkWedriveCapacityService",
            parentDeclaredEndpointCount: 2,
            new[]
            {
                (typeof(IWechatWorkInternalWedriveCapacityService), 0),
                (typeof(IWechatWorkProviderWedriveCapacityService), 0),
                (typeof(IWechatWorkThirdPartyWedriveCapacityService), 0),
            });

        // 高级功能账号管理族：官方仅自建应用开放（代开发/第三方标注「暂不支持」），
        // 父接口零端点 + 唯一自建子接口承载 3 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkWedriveVipService),
            parentImplementation: "WechatWorkWedriveVipService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalWedriveVipService), 3),
            });
    }

    /// <summary>族断言：父接口 IsAbstract + 指定端点数，子接口集合不漂移 + 指定端点数 + 注册组/继承契约。</summary>
    private static void AssertFamily(
        Type parent,
        string parentImplementation,
        int parentDeclaredEndpointCount,
        (Type Interface, int DeclaredEndpointCount)[] children)
    {
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(parentDeclaredEndpointCount, $"{parent.Name} 承载官方开放面收敛的端点数");

        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();
        assignable.Should().BeEquivalentTo(children.Select(c => c.Interface),
            $"{parent.Name} 继承链子接口集合不得漂移（官方未开放的应用类型不得补子接口）");

        foreach (var (child, endpointCount) in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(WedriveRegistryGroupName,
                $"{child.Name} 必须挂 {WedriveRegistryGroupName} 注册组（Wedrive 模块共用 Add{WedriveRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementation,
                $"{child.Name} 必须继承父接口生成实现类 {parentImplementation}");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(endpointCount, $"{child.Name} 为空标记子接口，不承载差异端点");
        }
    }

    // ------------------------------------------------------------------
    // WR3：令牌绑定——微盘域 22 个接口统一 AccessToken 路由键 + Query 注入
    //（归属域键由 WechatTokenOwnerContractGuards 全局锁定，此处不重复）。
    // ------------------------------------------------------------------

    [Fact]
    public void WedriveTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkWedriveSpaceService),
            typeof(IWechatWorkInternalWedriveSpaceService),
            typeof(IWechatWorkProviderWedriveSpaceService),
            typeof(IWechatWorkThirdPartyWedriveSpaceService),
            typeof(IWechatWorkWedriveSpaceAclService),
            typeof(IWechatWorkInternalWedriveSpaceAclService),
            typeof(IWechatWorkProviderWedriveSpaceAclService),
            typeof(IWechatWorkThirdPartyWedriveSpaceAclService),
            typeof(IWechatWorkWedriveFileService),
            typeof(IWechatWorkInternalWedriveFileService),
            typeof(IWechatWorkProviderWedriveFileService),
            typeof(IWechatWorkThirdPartyWedriveFileService),
            typeof(IWechatWorkWedriveFileAclService),
            typeof(IWechatWorkInternalWedriveFileAclService),
            typeof(IWechatWorkProviderWedriveFileAclService),
            typeof(IWechatWorkThirdPartyWedriveFileAclService),
            typeof(IWechatWorkWedriveCapacityService),
            typeof(IWechatWorkInternalWedriveCapacityService),
            typeof(IWechatWorkProviderWedriveCapacityService),
            typeof(IWechatWorkThirdPartyWedriveCapacityService),
            typeof(IWechatWorkWedriveVipService),
            typeof(IWechatWorkInternalWedriveVipService),
        };

        accessTokenInterfaces.Should().HaveCount(22,
            "微盘域六族 = 管理空间族 4 接口 + 管理空间权限族 4 接口 + 管理文件族 4 接口 + 管理文件权限族 4 接口 + 版本和容量管理族 4 接口 + 高级功能账号管理族 2 接口（仅自建）");

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（自建为应用自身令牌，第三方/代开发为授权企业级令牌）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    // ------------------------------------------------------------------
    // WR4：请求/响应 DTO 全量登记进 AOT JSON 上下文（SerializerClassName 统一 Wedrive）。
    // ------------------------------------------------------------------

    [Fact]
    public void WedriveDataModels_ShouldBeRegisteredInJsonContext()
    {
        var wedriveContext = WedriveJsonContext.Default;

        var domainTypes = typeof(CreateWedriveSpaceRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Wedrive"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 WedriveJsonContext 且 SerializerClassName 统一为 Wedrive
        //（生成物 WedriveJsonContext 自身亦落同命名空间，按 JsonSerializerContext 派生类型排除）。
        domainTypes.Should().HaveCount(60,
            "微盘模块契约面类型数漂移须先核对官方文档再同批调整本守卫（管理空间族 9：新建 2 + 重命名 1 + 解散 1 + 获取空间信息（旧版）3；" +
            "管理空间权限族 6：添加 1 + 移除 1 + 安全设置 1 + 获取邀请链接 2 + 获取空间信息（新版）1；空间共用嵌套 3；" +
            "管理文件族 22：列表 2 + 上传 2 + 分块上传 5 + 下载 2 + 新建 2 + 重命名 2 + 移动 2 + 删除 1 + 获取文件信息 2，文件共用嵌套 2；" +
            "管理文件权限族 12：新增 1 + 删除 1 + 分享设置 1 + 获取分享链接 2 + 获取文件权限信息 2 + 修改文件安全设置 1，权限共用嵌套 4；" +
            "版本和容量管理族 2（空请求体无请求 DTO）：专业版信息 1 + 容量信息 1；高级功能账号管理族 6：分配 2 + 取消 2 + 列表 2；" +
            "文件权限成员复用空间成员结构不另建类型）");

        foreach (var type in domainTypes)
        {
            wedriveContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于微盘域命名空间，必须登记进 WedriveJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Wedrive",
                $"{type.Name} 的 SerializerClassName 必须为微盘域段 Wedrive");
        }

        // 端点级请求/响应 DTO 落位抽查（关键端点契约面清单）。
        var endpointContractTypes = new Type[]
        {
            // 管理空间族。
            typeof(CreateWedriveSpaceRequest), typeof(CreateWedriveSpaceResponse),
            typeof(RenameWedriveSpaceRequest),
            typeof(DismissWedriveSpaceRequest),
            typeof(GetWedriveSpaceInfoRequest), typeof(GetWedriveSpaceInfoResponse),
            // 管理空间权限族。
            typeof(AddWedriveSpaceAclRequest),
            typeof(DelWedriveSpaceAclRequest),
            typeof(SetWedriveSpaceSettingRequest),
            typeof(GetWedriveSpaceShareUrlRequest), typeof(GetWedriveSpaceShareUrlResponse),
            typeof(GetWedriveNewSpaceInfoRequest), typeof(GetWedriveNewSpaceInfoResponse),
            // 管理文件族。
            typeof(GetWedriveFileListRequest), typeof(GetWedriveFileListResponse),
            typeof(UploadWedriveFileRequest), typeof(UploadWedriveFileResponse),
            typeof(InitWedriveFileUploadRequest), typeof(InitWedriveFileUploadResponse),
            typeof(UploadWedriveFilePartRequest),
            typeof(FinishWedriveFileUploadRequest), typeof(FinishWedriveFileUploadResponse),
            typeof(DownloadWedriveFileRequest), typeof(DownloadWedriveFileResponse),
            typeof(CreateWedriveFileRequest), typeof(CreateWedriveFileResponse),
            typeof(RenameWedriveFileRequest), typeof(RenameWedriveFileResponse),
            typeof(MoveWedriveFileRequest), typeof(MoveWedriveFileResponse),
            typeof(DeleteWedriveFileRequest),
            typeof(GetWedriveFileInfoRequest), typeof(GetWedriveFileInfoResponse),
            // 管理文件权限族。
            typeof(AddWedriveFileAclRequest),
            typeof(DelWedriveFileAclRequest),
            typeof(SetWedriveFileSettingRequest),
            typeof(GetWedriveFileShareUrlRequest), typeof(GetWedriveFileShareUrlResponse),
            typeof(GetWedriveFilePermissionRequest), typeof(GetWedriveFilePermissionResponse),
            typeof(SetWedriveFileSecureSettingRequest),
            // 版本和容量管理族（空请求体，仅响应 DTO）。
            typeof(GetWedriveProInfoResponse), typeof(GetWedriveCapacityInfoResponse),
            // 高级功能账号管理族。
            typeof(BatchAddWedriveVipRequest), typeof(BatchAddWedriveVipResponse),
            typeof(BatchDelWedriveVipRequest), typeof(BatchDelWedriveVipResponse),
            typeof(ListWedriveVipRequest), typeof(ListWedriveVipResponse),
            // 共用嵌套对象。
            typeof(WedriveSpaceAclMember), typeof(WedriveSpaceAuthList),
            typeof(WedriveSpaceInfo), typeof(WedriveNewSpaceInfo), typeof(WedriveSpaceSecureSetting),
            typeof(WedriveFileInfo), typeof(WedriveFileList),
            typeof(WedriveFileShareRange), typeof(WedriveFileSecureSetting),
            typeof(WedriveFileInheritFatherAuth), typeof(WedriveFileWatermark),
        };
        domainTypes.Should().Contain(endpointContractTypes, "端点级请求/响应 DTO 必须落位于微盘域命名空间");
    }

    // ------------------------------------------------------------------
    // WR5：官方契约陷阱锁定——新旧获取空间信息分形态承载、共用嵌套结构、
    //      ban_share_external/enable_share_external 不同名与 spaceid 拼写形态。
    // ------------------------------------------------------------------

    [Fact]
    public void WedriveDataModels_ShouldLockOfficialContractTraps()
    {
        // 新旧「获取空间信息」响应结构不同构，分别以两个类型承载（新版额外返回 secure_setting）。
        typeof(GetWedriveSpaceInfoResponse).GetProperty(nameof(GetWedriveSpaceInfoResponse.SpaceInfo))!
            .PropertyType.Should().Be(typeof(WedriveSpaceInfo),
                "旧版 space_info 响应结构不含安全设置，必须以 WedriveSpaceInfo 承载");
        typeof(GetWedriveNewSpaceInfoResponse).GetProperty(nameof(GetWedriveNewSpaceInfoResponse.SpaceInfo))!
            .PropertyType.Should().Be(typeof(WedriveNewSpaceInfo),
                "新版 new_space_info 响应结构含 secure_setting，必须以 WedriveNewSpaceInfo 承载");
        typeof(WedriveSpaceInfo).GetProperty("SecureSetting").Should().BeNull(
            "旧版空间信息不承载 secure_setting（新版同名字段仅落 WedriveNewSpaceInfo）");
        typeof(WedriveNewSpaceInfo).GetProperty(nameof(WedriveNewSpaceInfo.SecureSetting))!
            .PropertyType.Should().Be(typeof(WedriveSpaceSecureSetting), "新版空间信息安全设置结构");

        // 新建空间 / 添加成员部门 / 移除成员部门请求与两版响应 auth_list 共用同一成员权限结构
        //（官方参数表同构；移除成员部门不携带 auth，由可空 + WhenWritingNull 保证不序列化）。
        typeof(CreateWedriveSpaceRequest).GetProperty(nameof(CreateWedriveSpaceRequest.AuthInfo))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "新建空间 auth_info 与添加/移除成员共用同一结构");
        typeof(AddWedriveSpaceAclRequest).GetProperty(nameof(AddWedriveSpaceAclRequest.AuthInfo))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "添加成员 auth_info 与新建空间/移除成员共用同一结构");
        typeof(DelWedriveSpaceAclRequest).GetProperty(nameof(DelWedriveSpaceAclRequest.AuthInfo))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "移除成员 auth_info 与新建空间/添加成员共用同一结构");
        typeof(WedriveSpaceAuthList).GetProperty(nameof(WedriveSpaceAuthList.AuthInfo))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "空间成员权限列表与请求侧共用同一成员结构");
        typeof(WedriveSpaceAclMember).GetProperty(nameof(WedriveSpaceAclMember.Auth))!
            .PropertyType.Should().Be(typeof(ulong?), "移除成员/部门官方请求不携带 auth，auth 必须可空");

        // 空间 ID 官方字段名作 spaceid（无下划线）——照抄勿「顺手修正」。
        JsonNameShouldBe(typeof(CreateWedriveSpaceResponse), nameof(CreateWedriveSpaceResponse.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(RenameWedriveSpaceRequest), nameof(RenameWedriveSpaceRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(DismissWedriveSpaceRequest), nameof(DismissWedriveSpaceRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(GetWedriveSpaceInfoRequest), nameof(GetWedriveSpaceInfoRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(AddWedriveSpaceAclRequest), nameof(AddWedriveSpaceAclRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(DelWedriveSpaceAclRequest), nameof(DelWedriveSpaceAclRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(GetWedriveSpaceShareUrlRequest), nameof(GetWedriveSpaceShareUrlRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(GetWedriveNewSpaceInfoRequest), nameof(GetWedriveNewSpaceInfoRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(WedriveSpaceInfo), nameof(WedriveSpaceInfo.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(WedriveNewSpaceInfo), nameof(WedriveNewSpaceInfo.Spaceid), "spaceid");

        // 邀请链接字段名与已退出空间成员字段名照抄官方原文。
        JsonNameShouldBe(typeof(GetWedriveSpaceShareUrlResponse), nameof(GetWedriveSpaceShareUrlResponse.SpaceShareUrl), "space_share_url");
        JsonNameShouldBe(typeof(WedriveSpaceAuthList), nameof(WedriveSpaceAuthList.QuitUserid), "quit_userid");
        JsonNameShouldBe(typeof(WedriveSpaceAuthList), nameof(WedriveSpaceAuthList.AuthInfo), "auth_info");

        // 「禁止文件分享到企业外」写入口（space_setting）与读取回显（secure_setting）官方字段名不同构。
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.BanShareExternal), "ban_share_external");
        JsonNameShouldBe(typeof(WedriveSpaceSecureSetting), nameof(WedriveSpaceSecureSetting.EnableShareExternal), "enable_share_external");

        // 安全设置可写字段名照抄官方原文（未填充字段保持原有状态由 WhenWritingNull 保证）。
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.EnableWatermark), "enable_watermark");
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.EnableConfidentialMode), "enable_confidential_mode");
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.ShareUrlNoApprove), "share_url_no_approve");
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.ShareUrlNoApproveDefaultAuth), "share_url_no_approve_default_auth");
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.DefaultFileScope), "default_file_scope");

        // ------------------------------------------------------------------
        // 管理文件族契约陷阱。
        // ------------------------------------------------------------------

        // 文件信息对象跨四处共用同一结构：file_list.item / file_info / 重命名响应 file / 移动响应 file_list.item。
        typeof(GetWedriveFileListResponse).GetProperty(nameof(GetWedriveFileListResponse.FileList))!
            .PropertyType.Should().Be(typeof(WedriveFileList),
                "file_list 响应官方为对象内含 item 数组（file_list.item），不得建成裸数组");
        typeof(MoveWedriveFileResponse).GetProperty(nameof(MoveWedriveFileResponse.FileList))!
            .PropertyType.Should().Be(typeof(WedriveFileList), "移动文件响应 file_list 与文件列表共用同一结构");
        typeof(WedriveFileList).GetProperty(nameof(WedriveFileList.Item))!
            .PropertyType.Should().Be(typeof(List<WedriveFileInfo>), "file_list.item 与获取文件信息/重命名响应共用同一文件结构");
        typeof(RenameWedriveFileResponse).GetProperty(nameof(RenameWedriveFileResponse.File))!
            .PropertyType.Should().Be(typeof(WedriveFileInfo), "重命名响应 file 字段与获取文件信息共用同一结构（官方字段名作 file 非 file_info）");
        typeof(GetWedriveFileInfoResponse).GetProperty(nameof(GetWedriveFileInfoResponse.FileInfo))!
            .PropertyType.Should().Be(typeof(WedriveFileInfo), "获取文件信息响应字段官方名作 file_info");

        // 文件列表分页字段名照抄官方原文：has_more / next_start（start+limit 形态，非 cursor+limit）。
        JsonNameShouldBe(typeof(GetWedriveFileListResponse), nameof(GetWedriveFileListResponse.HasMore), "has_more");
        JsonNameShouldBe(typeof(GetWedriveFileListResponse), nameof(GetWedriveFileListResponse.NextStart), "next_start");
        JsonNameShouldBe(typeof(GetWedriveFileListRequest), nameof(GetWedriveFileListRequest.Start), "start");
        JsonNameShouldBe(typeof(GetWedriveFileListRequest), nameof(GetWedriveFileListRequest.SortType), "sort_type");

        // 移动/删除文件的请求字段官方即作 fileid 但为数组形态（单数名复数承载，照抄勿改）。
        JsonNameShouldBe(typeof(MoveWedriveFileRequest), nameof(MoveWedriveFileRequest.Fileid), "fileid");
        typeof(MoveWedriveFileRequest).GetProperty(nameof(MoveWedriveFileRequest.Fileid))!
            .PropertyType.Should().Be(typeof(List<string>), "移动文件 fileid 官方为数组形态");
        JsonNameShouldBe(typeof(DeleteWedriveFileRequest), nameof(DeleteWedriveFileRequest.Fileid), "fileid");
        typeof(DeleteWedriveFileRequest).GetProperty(nameof(DeleteWedriveFileRequest.Fileid))!
            .PropertyType.Should().Be(typeof(List<string>), "删除文件 fileid 官方为数组形态");

        // 文件 ID 官方字段名作 fileid（无下划线）、目录字段作 fatherid——照抄勿「顺手修正」。
        JsonNameShouldBe(typeof(UploadWedriveFileResponse), nameof(UploadWedriveFileResponse.Fileid), "fileid");
        JsonNameShouldBe(typeof(InitWedriveFileUploadResponse), nameof(InitWedriveFileUploadResponse.Fileid), "fileid");
        JsonNameShouldBe(typeof(FinishWedriveFileUploadResponse), nameof(FinishWedriveFileUploadResponse.Fileid), "fileid");
        JsonNameShouldBe(typeof(CreateWedriveFileResponse), nameof(CreateWedriveFileResponse.Fileid), "fileid");
        JsonNameShouldBe(typeof(GetWedriveFileInfoRequest), nameof(GetWedriveFileInfoRequest.Fileid), "fileid");
        JsonNameShouldBe(typeof(RenameWedriveFileRequest), nameof(RenameWedriveFileRequest.Fileid), "fileid");
        JsonNameShouldBe(typeof(RenameWedriveFileRequest), nameof(RenameWedriveFileRequest.NewName), "new_name");
        JsonNameShouldBe(typeof(WedriveFileInfo), nameof(WedriveFileInfo.Fileid), "fileid");
        JsonNameShouldBe(typeof(WedriveFileInfo), nameof(WedriveFileInfo.FileName), "file_name");
        JsonNameShouldBe(typeof(WedriveFileInfo), nameof(WedriveFileInfo.Fatherid), "fatherid");
        JsonNameShouldBe(typeof(WedriveFileInfo), nameof(WedriveFileInfo.FileSize), "file_size");
        JsonNameShouldBe(typeof(WedriveFileInfo), nameof(WedriveFileInfo.FileType), "file_type");
        JsonNameShouldBe(typeof(WedriveFileInfo), nameof(WedriveFileInfo.FileStatus), "file_status");

        // 分块上传：秒传判定 hit_exist、上传凭证 upload_key、分块号 index（int 从 1 开始）。
        JsonNameShouldBe(typeof(InitWedriveFileUploadResponse), nameof(InitWedriveFileUploadResponse.HitExist), "hit_exist");
        JsonNameShouldBe(typeof(InitWedriveFileUploadResponse), nameof(InitWedriveFileUploadResponse.UploadKey), "upload_key");
        JsonNameShouldBe(typeof(InitWedriveFileUploadRequest), nameof(InitWedriveFileUploadRequest.BlockSha), "block_sha");
        JsonNameShouldBe(typeof(InitWedriveFileUploadRequest), nameof(InitWedriveFileUploadRequest.SkipPushCard), "skip_push_card");
        JsonNameShouldBe(typeof(UploadWedriveFilePartRequest), nameof(UploadWedriveFilePartRequest.UploadKey), "upload_key");
        JsonNameShouldBe(typeof(UploadWedriveFilePartRequest), nameof(UploadWedriveFilePartRequest.Index), "index");
        typeof(UploadWedriveFilePartRequest).GetProperty(nameof(UploadWedriveFilePartRequest.Index))!
            .PropertyType.Should().Be(typeof(int?), "分块号官方标注 int32（区别于域内其余 uint32 字段）");

        // 上传/下载互斥参数组：spaceid/fatherid 与 selected_ticket（下载为 fileid 与 selected_ticket）。
        JsonNameShouldBe(typeof(UploadWedriveFileRequest), nameof(UploadWedriveFileRequest.SelectedTicket), "selected_ticket");
        JsonNameShouldBe(typeof(UploadWedriveFileRequest), nameof(UploadWedriveFileRequest.FileBase64Content), "file_base64_content");
        JsonNameShouldBe(typeof(InitWedriveFileUploadRequest), nameof(InitWedriveFileUploadRequest.SelectedTicket), "selected_ticket");
        JsonNameShouldBe(typeof(DownloadWedriveFileRequest), nameof(DownloadWedriveFileRequest.SelectedTicket), "selected_ticket");

        // 下载文件响应：download_url 有效期 2 小时，Cookie 键值对随响应返回。
        JsonNameShouldBe(typeof(DownloadWedriveFileResponse), nameof(DownloadWedriveFileResponse.DownloadUrl), "download_url");
        JsonNameShouldBe(typeof(DownloadWedriveFileResponse), nameof(DownloadWedriveFileResponse.CookieName), "cookie_name");
        JsonNameShouldBe(typeof(DownloadWedriveFileResponse), nameof(DownloadWedriveFileResponse.CookieValue), "cookie_value");

        // ------------------------------------------------------------------
        // 管理文件权限族契约陷阱。
        // ------------------------------------------------------------------

        // 文件权限成员与空间成员共用同一结构（type/userid/departmentid/auth 同构；
        // 文件侧 auth 仅 1、type/departmentid 官方标注后续将废弃、acl_del 不携带 auth）。
        typeof(AddWedriveFileAclRequest).GetProperty(nameof(AddWedriveFileAclRequest.AuthInfo))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "文件权限新增成员 auth_info 复用空间成员结构");
        typeof(DelWedriveFileAclRequest).GetProperty(nameof(DelWedriveFileAclRequest.AuthInfo))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "文件权限删除成员 auth_info 复用空间成员结构");

        // 获取文件权限信息响应嵌套结构与字段名照抄官方原文（file_member_list 官方参数表列 obj、示例实为数组）。
        JsonNameShouldBe(typeof(GetWedriveFilePermissionResponse), nameof(GetWedriveFilePermissionResponse.ShareRange), "share_range");
        JsonNameShouldBe(typeof(GetWedriveFilePermissionResponse), nameof(GetWedriveFilePermissionResponse.SecureSetting), "secure_setting");
        JsonNameShouldBe(typeof(GetWedriveFilePermissionResponse), nameof(GetWedriveFilePermissionResponse.InheritFatherAuth), "inherit_father_auth");
        JsonNameShouldBe(typeof(GetWedriveFilePermissionResponse), nameof(GetWedriveFilePermissionResponse.FileMemberList), "file_member_list");
        JsonNameShouldBe(typeof(GetWedriveFilePermissionResponse), nameof(GetWedriveFilePermissionResponse.CoAuthList), "co_auth_list");
        JsonNameShouldBe(typeof(GetWedriveFilePermissionResponse), nameof(GetWedriveFilePermissionResponse.Watermark), "watermark");
        typeof(GetWedriveFilePermissionResponse).GetProperty(nameof(GetWedriveFilePermissionResponse.FileMemberList))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "file_member_list 官方示例实为成员数组");
        typeof(WedriveFileInheritFatherAuth).GetProperty(nameof(WedriveFileInheritFatherAuth.AuthList))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "父路径继承权限返回示例实为 auth_list 成员数组");
        typeof(WedriveFileInheritFatherAuth).GetProperty(nameof(WedriveFileInheritFatherAuth.MemberList))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "参数表列 member_list，与 auth_list 双形态均承载");

        // 分享范围与文件安全设置字段名照抄官方原文（corp_*_auth 取值含 255 无权限或需审批）。
        JsonNameShouldBe(typeof(WedriveFileShareRange), nameof(WedriveFileShareRange.EnableCorpInternal), "enable_corp_internal");
        JsonNameShouldBe(typeof(WedriveFileShareRange), nameof(WedriveFileShareRange.CorpInternalAuth), "corp_internal_auth");
        JsonNameShouldBe(typeof(WedriveFileShareRange), nameof(WedriveFileShareRange.EnableCorpExternal), "enable_corp_external");
        JsonNameShouldBe(typeof(WedriveFileShareRange), nameof(WedriveFileShareRange.CorpExternalAuth), "corp_external_auth");
        JsonNameShouldBe(typeof(WedriveFileShareRange), nameof(WedriveFileShareRange.CorpInternalApproveOnlyByAdmin), "corp_internal_approve_only_by_admin");
        JsonNameShouldBe(typeof(WedriveFileShareRange), nameof(WedriveFileShareRange.CorpExternalApproveOnlyByAdmin), "corp_external_approve_only_by_admin");
        JsonNameShouldBe(typeof(WedriveFileSecureSetting), nameof(WedriveFileSecureSetting.EnableReadonlyCopy), "enable_readonly_copy");
        JsonNameShouldBe(typeof(WedriveFileSecureSetting), nameof(WedriveFileSecureSetting.ModifyOnlyByAdmin), "modify_only_by_admin");
        JsonNameShouldBe(typeof(WedriveFileSecureSetting), nameof(WedriveFileSecureSetting.EnableReadonlyComment), "enable_readonly_comment");
        JsonNameShouldBe(typeof(WedriveFileSecureSetting), nameof(WedriveFileSecureSetting.BanShareExternal), "ban_share_external");

        // 水印结构跨读写两端共用：写入口（file_secure_setting）仅开放 4 字段，
        // force_by_admin / force_by_space_admin 为读取回显字段（可空 + WhenWritingNull 保证不序列化）。
        typeof(SetWedriveFileSecureSettingRequest).GetProperty(nameof(SetWedriveFileSecureSettingRequest.Watermark))!
            .PropertyType.Should().Be(typeof(WedriveFileWatermark), "修改文件安全设置 watermark 与读取回显共用同一结构");
        JsonNameShouldBe(typeof(WedriveFileWatermark), nameof(WedriveFileWatermark.Text), "text");
        JsonNameShouldBe(typeof(WedriveFileWatermark), nameof(WedriveFileWatermark.MarginType), "margin_type");
        JsonNameShouldBe(typeof(WedriveFileWatermark), nameof(WedriveFileWatermark.ShowVisitorName), "show_visitor_name");
        JsonNameShouldBe(typeof(WedriveFileWatermark), nameof(WedriveFileWatermark.ForceByAdmin), "force_by_admin");
        JsonNameShouldBe(typeof(WedriveFileWatermark), nameof(WedriveFileWatermark.ShowText), "show_text");
        JsonNameShouldBe(typeof(WedriveFileWatermark), nameof(WedriveFileWatermark.ForceBySpaceAdmin), "force_by_space_admin");

        // 文件分享链接字段名 share_url（区别于空间邀请链接 space_share_url）。
        JsonNameShouldBe(typeof(GetWedriveFileShareUrlResponse), nameof(GetWedriveFileShareUrlResponse.ShareUrl), "share_url");

        // ------------------------------------------------------------------
        // 版本和容量管理族 + 高级功能账号管理族契约陷阱。
        // ------------------------------------------------------------------

        // 专业版信息与容量信息字段名照抄官方原文（capacity 单位为 B）。
        JsonNameShouldBe(typeof(GetWedriveProInfoResponse), nameof(GetWedriveProInfoResponse.IsPro), "is_pro");
        JsonNameShouldBe(typeof(GetWedriveProInfoResponse), nameof(GetWedriveProInfoResponse.TotalVipAcctNum), "total_vip_acct_num");
        JsonNameShouldBe(typeof(GetWedriveProInfoResponse), nameof(GetWedriveProInfoResponse.UseVipAcctNum), "use_vip_acct_num");
        JsonNameShouldBe(typeof(GetWedriveProInfoResponse), nameof(GetWedriveProInfoResponse.ProExpireTime), "pro_expire_time");
        JsonNameShouldBe(typeof(GetWedriveCapacityInfoResponse), nameof(GetWedriveCapacityInfoResponse.TotalCapacityForAll), "total_capacity_for_all");
        JsonNameShouldBe(typeof(GetWedriveCapacityInfoResponse), nameof(GetWedriveCapacityInfoResponse.TotalCapacityForVip), "total_capacity_for_vip");

        // 分配/取消高级功能账号响应形态同构（succ/fail 双列表），但分属两端点各自承载 DTO。
        JsonNameShouldBe(typeof(BatchAddWedriveVipResponse), nameof(BatchAddWedriveVipResponse.SuccUseridList), "succ_userid_list");
        JsonNameShouldBe(typeof(BatchAddWedriveVipResponse), nameof(BatchAddWedriveVipResponse.FailUseridList), "fail_userid_list");
        JsonNameShouldBe(typeof(BatchDelWedriveVipResponse), nameof(BatchDelWedriveVipResponse.SuccUseridList), "succ_userid_list");
        JsonNameShouldBe(typeof(BatchDelWedriveVipResponse), nameof(BatchDelWedriveVipResponse.FailUseridList), "fail_userid_list");
        typeof(BatchAddWedriveVipRequest).GetProperty(nameof(BatchAddWedriveVipRequest.UseridList))!
            .PropertyType.Should().Be(typeof(List<string>), "分配账号 userid_list 官方为数组形态（单次上限 100）");

        // 账号列表 cursor + limit 分页（has_more / next_cursor），与文件列表的 start + limit 形态并存于同域。
        JsonNameShouldBe(typeof(ListWedriveVipRequest), nameof(ListWedriveVipRequest.Cursor), "cursor");
        JsonNameShouldBe(typeof(ListWedriveVipRequest), nameof(ListWedriveVipRequest.Limit), "limit");
        JsonNameShouldBe(typeof(ListWedriveVipResponse), nameof(ListWedriveVipResponse.HasMore), "has_more");
        JsonNameShouldBe(typeof(ListWedriveVipResponse), nameof(ListWedriveVipResponse.NextCursor), "next_cursor");
        JsonNameShouldBe(typeof(ListWedriveVipResponse), nameof(ListWedriveVipResponse.UseridList), "userid_list");
    }

    /// <summary>路由表断言：方法必须存在、必须声明对应 HTTP 方法特性且路由与官方契约一致。</summary>
    private static void AssertRoutes((Type Interface, string Method, Type HttpAttribute, string Route)[] routes)
    {
        foreach (var (iface, method, httpAttribute, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>JSON 字段名断言：属性映射的官方字段名必须与官方原文一致（拼写差异属官方契约）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        jsonName.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}
