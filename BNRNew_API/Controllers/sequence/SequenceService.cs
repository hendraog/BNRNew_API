using Microsoft.EntityFrameworkCore;
using BNRNew_API.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.Sqlite;
using System.Text.RegularExpressions;
using BNRNew_API.Controllers.golongan;
using System.Numerics;
using BNRNew_API.config;

namespace BNRNew_API.Controllers.golonganplat
{
    public class SequenceService : ISequenceService
    {
        MyDBContext ctx;
        IGolonganService golonganService;

        public SequenceService(MyDBContext ctx) { 
            this.ctx = ctx;
        }

        public async Task<string> getSequence(SequenceKey key)
        {
            var sequence = await ctx.sequence.Where(x => x.key == key).SingleOrDefaultAsync();
            if (sequence == null)
            {
                sequence = new Sequence()
                {
                    key = key,
                    last_value = 1,
                    last_used = DateTime.UtcNow
                };
                await ctx.sequence.AddAsync(sequence);
                await ctx.SaveChangesAsync();
            }
            else
            {
                string s1 = $"{DateTime.UtcNow:yyyyMMdd}";
                string s2 = $"{sequence.last_used:yyyyMMdd}";

                if (s1.Equals(s2))
                    sequence.last_value = sequence.last_value + 1;
                else
                    sequence.last_value = 1;

                sequence.last_used = DateTime.UtcNow;
                ctx.sequence.Update(sequence);
                await ctx.SaveChangesAsync();
            }

            if (key == SequenceKey.TICKET)
            {
                var counter = sequence.last_value.ToString().PadLeft(6, '0');
                return $"{DateTime.Now:yyyyMMdd}-{counter}";
            }
            else
            {
                var counter = sequence.last_value.ToString().PadLeft(3, '0');
                return $"CAR{counter}-{DateTime.Now:yyyyMMdd}";
            }
        }
    }

    public interface ISequenceService
    {
        public Task<string> getSequence(SequenceKey key);
    }
}
