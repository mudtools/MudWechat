// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedrive;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微盘」模块管理文件域公共 SDK
/// （获取文件列表 + 上传文件 + 文件分块上传（初始化/分块/完成） + 下载文件 + 新建文件夹/文档 +
/// 重命名文件 + 移动文件 + 删除文件 + 获取文件信息）。
/// <para>
/// 官方对三类应用开放一致的端点面，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedriveFileService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedriveFileService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedriveFileService"/>。
/// </para>
/// <para>
/// 官方契约陷阱：「文件分块上传」官方单文档页承载 3 条路由（file_upload_init / file_upload_part /
/// file_upload_finish）；文件分页用 start + limit（start 首次填 0、后续填上一次返回的 next_start），
/// 区别于本仓多数域的 cursor + limit。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)</c> 建立「应用 + 企业」作用域后再调用。
/// 官方权限口径：自建应用需配置到「可调用应用」列表中，使用对应应用 secret 获取的 accesstoken 调用；
/// 第三方应用与服务商代开发应用需具有「微盘」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedriveFileService
{
    /// <summary>
    /// 获取文件列表
    /// <para>获取指定目录下的文件列表（含文件夹与微文档），分批拉取。</para>
    /// <para>官方限制：limit 分批拉取最大文件数不超过 1000；start 首次填 0，后续填上一次请求返回的
    /// next_start；fatherid 为当前目录的 fileid，根目录时填空间 spaceid。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedriveFileListRequest"/>：spaceid / fatherid / sort_type / start / limit，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>翻页标记（has_more / next_start）与文件信息列表（file_list.item：fileid / file_name / spaceid / fatherid / file_size / ctime / mtime / file_type / file_status / sha / md5 / url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93657"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95859"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96847"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_list")]
    Task<GetWedriveFileListResponse> GetFileListAsync(
        [Body] GetWedriveFileListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传文件
    /// <para>向指定位置上传文件（整文件 base64 承载，上限 10M；更大文件请用文件分块上传）。</para>
    /// <para>官方限制：spaceid/fatherid 和 selected_ticket 必须填且仅填其中一组参数；
    /// file_base64_content 只填文件内容的 Base64、不带数据类型描述前缀，文件大小上限 10M；
    /// 文件名最多 255 个字符（英文算 1 个，汉字算 2 个）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UploadWedriveFileRequest"/>：spaceid / fatherid / selected_ticket / file_name / file_base64_content）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建文件的 fileid。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97880"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97951"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97914"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_upload")]
    Task<UploadWedriveFileResponse> UploadFileAsync(
        [Body] UploadWedriveFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分块上传初始化
    /// <para>文件分块上传的第一步：初始化上传任务并获取分块上传凭证（命中秒传时直接完成）。</para>
    /// <para>官方限制：文件按 2M（2097152 字节）固定分块；block_sha 为按分块顺序的文件分块累积 sha1；
    /// size 最大支持 20G；spaceid/fatherid 和 selected_ticket 必须填且仅填其中一组参数；
    /// 命中秒传（hit_exist = true）时返回 fileid，此时上传流程完成、无需继续分块上传。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="InitWedriveFileUploadRequest"/>：spaceid / fatherid / selected_ticket / file_name / size / block_sha / skip_push_card）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否命中秒传（hit_exist）、分块上传凭证（upload_key，不命中秒传时返回）与文件 fileid（命中秒传时返回）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98004"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98005"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98007"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_upload_init")]
    Task<InitWedriveFileUploadResponse> InitFileUploadAsync(
        [Body] InitWedriveFileUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分块上传文件
    /// <para>文件分块上传的第二步：逐块上传文件内容（可与 <see cref="FinishFileUploadAsync"/> 配合完成整个上传流程）。</para>
    /// <para>官方限制：文件内容按 2M 分块，index 分块号从 1 开始；file_base64_content 只填该分块内容的
    /// Base64；分块可并发上传，官方建议并发数不超过 10。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UploadWedriveFilePartRequest"/>：upload_key / index / file_base64_content，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98004"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98005"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98007"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_upload_part")]
    Task<WechatWorkResponse> UploadFilePartAsync(
        [Body] UploadWedriveFilePartRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分块上传完成
    /// <para>文件分块上传的最后一步：标记上传完成并换取文件 fileid。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="FinishWedriveFileUploadRequest"/>：upload_key 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文件 fileid。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98004"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98005"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98007"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_upload_finish")]
    Task<FinishWedriveFileUploadResponse> FinishFileUploadAsync(
        [Body] FinishWedriveFileUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载文件
    /// <para>获取文件下载链接与下载请求所需的 Cookie（仅支持普通文件，不支持文件夹或微文档）。</para>
    /// <para>官方限制：fileid 和 selected_ticket 必须填且仅填其中一组参数；
    /// download_url 有效期为 2 个小时。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DownloadWedriveFileRequest"/>：fileid / selected_ticket）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>下载请求 url（download_url，有效期 2 小时）与下载请求 Cookie（cookie_name / cookie_value）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97881"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97953"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97915"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_download")]
    Task<DownloadWedriveFileResponse> DownloadFileAsync(
        [Body] DownloadWedriveFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 新建文件夹/文档
    /// <para>在指定目录下新建文件夹或微文档（文档/表格）。</para>
    /// <para>官方限制：file_type 取值 1:文件夹 3:文档(文档) 4:文档(表格)；
    /// 文件名最多 255 个字符（英文算 1 个，汉字算 2 个）；fatherid 根目录时填空间 spaceid。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateWedriveFileRequest"/>：spaceid / fatherid / file_type / file_name，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建文件的 fileid 与文档访问链接（url，仅新建文档时返回）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97882"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97954"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97916"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_create")]
    Task<CreateWedriveFileResponse> CreateFileAsync(
        [Body] CreateWedriveFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重命名文件
    /// <para>重命名指定文件/文件夹/微文档。</para>
    /// <para>官方限制：new_name 最多 255 个字符（英文算 1 个，汉字算 2 个）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="RenameWedriveFileRequest"/>：fileid / new_name，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>重命名后的文件信息（file：fileid / file_name / spaceid / fatherid / file_size / ctime / mtime / file_type / file_status / sha / md5）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97883"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97955"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97917"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_rename")]
    Task<RenameWedriveFileResponse> RenameFileAsync(
        [Body] RenameWedriveFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移动文件
    /// <para>将一个或多个文件移动到指定目录。</para>
    /// <para>官方限制：fileid 为数组、支持批量移动；replace 为目标目录存在重名文件时是否覆盖
    /// （true 覆盖；false 冲突重命名，如 xxx(1).txt、xxx(1).doc）；fatherid 根目录时填空间 spaceid。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MoveWedriveFileRequest"/>：fatherid / replace / fileid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>移动后的文件信息列表（file_list.item）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97884"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97956"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97918"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_move")]
    Task<MoveWedriveFileResponse> MoveFileAsync(
        [Body] MoveWedriveFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除文件
    /// <para>删除一个或多个文件/文件夹/微文档。</para>
    /// <para>官方限制：fileid 为数组、支持批量删除。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteWedriveFileRequest"/>：fileid 官方必填，数组形态）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97885"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97957"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97919"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_delete")]
    Task<WechatWorkResponse> DeleteFileAsync(
        [Body] DeleteWedriveFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文件信息
    /// <para>获取单个文件/文件夹/微文档的详情（基础信息 + sha/md5 校验值 + 微文档访问链接）。</para>
    /// <para>官方限制：sha/md5 可用于校验与上传文件是否一致或避免重复上传；经文件分块上传的文件
    /// md5 无效；url 仅微文档类型返回访问链接。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedriveFileInfoRequest"/>：fileid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文件信息（file_info：fileid / file_name / spaceid / fatherid / file_size / ctime / mtime / file_type / file_status / sha / md5 / url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97886"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97958"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97920"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_info")]
    Task<GetWedriveFileInfoResponse> GetFileInfoAsync(
        [Body] GetWedriveFileInfoRequest request,
        CancellationToken cancellationToken = default);
}
