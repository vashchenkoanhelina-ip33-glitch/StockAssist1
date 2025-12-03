using Microsoft.EntityFrameworkCore;
using StockAssist.Web.Data;
using StockAssist.Web.Models;

namespace StockAssist.Web.Services
{
    public class WarehouseAllocator3D
    {
        private readonly ApplicationDbContext db;
        public WarehouseAllocator3D(ApplicationDbContext d) => db = d;

        private bool IsFree(int wid, int x, int y, int z, int w, int d, int h)
        {
            return !db.WarehouseCells.Any(c =>
                c.WarehouseId == wid && c.IsOccupied &&
                c.X >= x && c.X < x + w &&
                c.Y >= y && c.Y < y + d &&
                c.Z >= z && c.Z < z + h);
        }

        public (int x, int y, int z, int wid)? Find(int w, int d, int h)
        {
            foreach (var wh in db.Warehouses.AsNoTracking())
                for (int x = 0; x <= wh.Width - w; x++)
                    for (int y = 0; y <= wh.Depth - d; y++)
                        for (int z = 0; z <= wh.Height - h; z++)
                            if (IsFree(wh.Id, x, y, z, w, d, h))
                                return (x, y, z, wh.Id);
            return null;
        }

        public (int x, int y, int z)? FindInWarehouse(int warehouseId, int w, int d, int h)
        {
            var wh = db.Warehouses.AsNoTracking().FirstOrDefault(x => x.Id == warehouseId);
            if (wh == null) return null;

            for (int x = 0; x <= wh.Width - w; x++)
                for (int y = 0; y <= wh.Depth - d; y++)
                    for (int z = 0; z <= wh.Height - h; z++)
                        if (IsFree(wh.Id, x, y, z, w, d, h))
                            return (x, y, z);

            return null;
        }

        public void Occupy(int wid, int x, int y, int z, int w, int d, int h)
        {
            var cells = new List<WarehouseCell>();
            for (int i = 0; i < w; i++)
                for (int j = 0; j < d; j++)
                    for (int k = 0; k < h; k++)
                        cells.Add(new WarehouseCell
                        {
                            WarehouseId = wid,
                            X = x + i,
                            Y = y + j,
                            Z = z + k,
                            IsOccupied = true
                        });

            db.WarehouseCells.AddRange(cells);
        }
    }
}
