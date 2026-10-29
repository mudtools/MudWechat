// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data;

/// <summary>
/// <see cref="IFreeSql"/> 的装配与建表（方案 §5.1）。
/// </summary>
/// <remarks>
/// <para>
/// <b>单例</b>：FreeSql 的 <c>IFreeSql</c> 本身线程安全，进程内一份即可；连接由内部池按需取用。
/// </para>
/// <para>
/// <b>为何显式 <c>SyncStructure</c> 而不开 <c>UseAutoSyncStructure(true)</c></b>：后者把「建表」挂到
/// 每个实体的首次查询上，建表时机取决于访问顺序，出问题时的日志非常难读。显式在启动期一次建全 9 张表，
/// 再补索引，行为可预测。代价与自动同步一致 —— <b>字段改名会丢列</b>，生产环境必须换迁移脚本（README 已标注）。
/// </para>
/// </remarks>
public static class FreeSqlProvider
{
    /// <summary>本 Demo 的全部实体（建表清单 + <c>/healthz</c> 行数探针共用，避免两处漂移）。</summary>
    public static readonly IReadOnlyList<Type> EntityTypes = new[]
    {
        typeof(OrgDepartment),
        typeof(OrgMember),
        typeof(OrgMemberDepartment),
        typeof(OrgSyncRun),
        typeof(OrgChangeLog),
        typeof(OaTemplate),
        typeof(OaTemplateVersion),
        typeof(OaTemplateControl),
        typeof(OaTemplateRegistry),
    };

    /// <summary>库内表名（与实体上的 <c>[Table(Name=…)]</c> 一一对应，供健康检查回显行数）。</summary>
    public static readonly IReadOnlyList<string> TableNames = new[]
    {
        "org_department",
        "org_member",
        "org_member_department",
        "org_sync_run",
        "org_change_log",
        "oa_template",
        "oa_template_version",
        "oa_template_control",
        "oa_template_registry",
    };

    /// <summary>构造 <see cref="IFreeSql"/>（不建表；建表见 <see cref="Initialize"/>）。</summary>
    public static IFreeSql Build(string connectionString) => new FreeSqlBuilder()
        .UseConnectionString(DataType.Sqlite, connectionString)
        .UseAutoSyncStructure(false)
        .Build();

    /// <summary>
    /// 启动期初始化：WAL + NORMAL 同步级别 + 建表 + 建索引。
    /// </summary>
    /// <remarks>
    /// WAL 是「后台全量同步写库」与「读端点并发查询」能同时进行的前提；<c>synchronous=NORMAL</c>
    /// 在 WAL 下仍是崩溃安全（丢失窗口只覆盖最后一次提交），对 Demo 的写入量没有收益损失。
    /// </remarks>
    public static void Initialize(IFreeSql fsql)
    {
        fsql.Ado.ExecuteNonQuery("PRAGMA journal_mode=WAL;");
        fsql.Ado.ExecuteNonQuery("PRAGMA synchronous=NORMAL;");

        fsql.CodeFirst.SyncStructure(EntityTypes.ToArray());

        foreach (var ddl in IndexDdl)
        {
            fsql.Ado.ExecuteNonQuery(ddl);
        }
    }

    /// <summary>
    /// 索引清单（方案 §5.2 的「索引」注释行）：直接写 DDL 而不用 FreeSql 的 <c>[Index]</c> 特性，
    /// 因为列名是显式映射的 snake_case，特性里的字段名与列名口径容易错位。
    /// </summary>
    private static readonly string[] IndexDdl =
    {
        "CREATE INDEX IF NOT EXISTS ix_org_department_parent ON org_department(parent_id);",
        "CREATE INDEX IF NOT EXISTS ix_org_member_name ON org_member(name);",
        "CREATE INDEX IF NOT EXISTS ix_org_member_main_dept ON org_member(main_dept_id);",
        "CREATE INDEX IF NOT EXISTS ix_org_member_department_dept ON org_member_department(dept_id);",
        "CREATE UNIQUE INDEX IF NOT EXISTS ux_oa_template_version ON oa_template_version(template_id, version);",
        "CREATE INDEX IF NOT EXISTS ix_oa_template_control_version ON oa_template_control(template_id, version);",
        "CREATE INDEX IF NOT EXISTS ix_org_change_log_entity ON org_change_log(entity_type, entity_id, kind, occurred_at);",
        "CREATE INDEX IF NOT EXISTS ix_org_sync_run_kind_status ON org_sync_run(kind, status);",
    };
}
