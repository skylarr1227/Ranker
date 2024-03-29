﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ranker
{
    public interface IDatabase
    {
        /// <summary>
        /// Gets ranks for all members.
        /// </summary>
        Task<List<Rank>> GetAsync();

        /// <summary>
        /// Gets rank for a particular member.
        /// </summary>
        /// <param name="userId">The member ID.</param>
        /// <param name="guildId">The guild (server) ID.</param>
        Task<Rank> GetAsync(ulong userId, ulong guildId);

        /// <summary>
        /// Gets all roles.
        /// </summary>
        Task<List<Role>> GetRolesAsync();

        /// <summary>
        /// Modify an existing rank or add a new rank.
        /// </summary>
        /// <param name="newRank">The new rank.</param>
        Task UpsertAsync(ulong userId, ulong guildId, Rank newRank);

        /// <summary>
        /// Modify an existing role or add a new role.
        /// </summary>
        /// <param name="level">The role level.</param>
        /// <param name="id">The role id.</param>
        Task UpsertAsync(ulong level, ulong id);
    }
}
