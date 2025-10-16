using System;
using System.Collections.Generic;
using System.Linq;

enum VehicleType
{
    Mobil,
    Motor
}

class Vehicle
{
    public string RegistrationNumber { get; set; } = "";
    public string Colour { get; set; } = "";
    public VehicleType Type { get; set; }
    public DateTime CheckIn { get; set; }
}

class ParkingLot
{
    private readonly Vehicle?[] slots;
    private readonly decimal feePerHour;

    public ParkingLot(int size, decimal feePerHour = 5000m)
    {
        if (size <= 0) throw new ArgumentException("Size must be positive", nameof(size));
        slots = new Vehicle?[size];
        this.feePerHour = feePerHour;
    }

    public int Capacity => slots.Length;

    public int Park(Vehicle v)
    {
        if (v == null) throw new ArgumentNullException(nameof(v));
        if (slots.Any(s => s != null && string.Equals(s!.RegistrationNumber, v.RegistrationNumber, StringComparison.OrdinalIgnoreCase)))
        {
            return -2;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                v.CheckIn = DateTime.Now;
                slots[i] = v;
                return i + 1;
            }
        }
        return -1;
    }

    public (bool ok, decimal fee, int hours) Leave(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > slots.Length) return (false, 0m, 0);
        int idx = slotNumber - 1;
        var v = slots[idx];
        if (v == null) return (false, 0m, 0);

        var now = DateTime.Now;
        var elapsed = now - v.CheckIn;
        int hours = (int)Math.Ceiling(elapsed.TotalHours);
        if (hours < 1) hours = 1;
        decimal fee = hours * feePerHour;

        slots[idx] = null;
        return (true, fee, hours);
    }

    public IEnumerable<(int slot, Vehicle v)> OccupiedSlots()
    {
        for (int i = 0; i < slots.Length; i++)
            if (slots[i] != null) yield return (i + 1, slots[i]!);
    }

    public int CountOccupied() => slots.Count(s => s != null);
    public int CountAvailable() => slots.Count(s => s == null);
    public IEnumerable<Vehicle> AllVehicles() => slots.Where(s => s != null).Select(s => s!);

    public int CountByType(VehicleType type) =>
        AllVehicles().Count(v => v.Type == type);

    public List<string> RegistrationNumbersByColour(string colour) =>
        OccupiedSlots()
            .Where(x => string.Equals(x.v.Colour, colour, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.slot)
            .Select(x => x.v.RegistrationNumber)
            .ToList();

    public List<int> SlotNumbersByColour(string colour) =>
        OccupiedSlots()
            .Where(x => string.Equals(x.v.Colour, colour, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.slot)
            .Select(x => x.slot)
            .ToList();

    public int? SlotNumberForRegistration(string reg)
    {
        if (string.IsNullOrWhiteSpace(reg)) return null;
        for (int i = 0; i < slots.Length; i++)
        {
            var s = slots[i];
            if (s != null && string.Equals(s.RegistrationNumber, reg, StringComparison.OrdinalIgnoreCase))
                return i + 1;
        }
        return null;
    }

    public List<string> RegistrationNumbersByOddEven(bool wantOdd)
    {
        var list = new List<(int slot, string reg)>();
        foreach (var x in OccupiedSlots())
        {
            var reg = x.v.RegistrationNumber;
            char? lastDigit = null;
            for (int i = reg.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(reg[i])) { lastDigit = reg[i]; break; }
            }
            if (lastDigit != null)
            {
                int d = (int)char.GetNumericValue(lastDigit.Value);
                if ((d % 2 == 1) == wantOdd) list.Add((x.slot, reg));
            }
        }
        return list.OrderBy(r => r.slot).Select(r => r.reg).ToList();
    }

        public string Status()
        {
            var lines = new List<string>();
            lines.Add("Slot No. | Registration No | Colour    | Type  | CheckIn");
            lines.Add(new string('-', 72));
            foreach (var x in OccupiedSlots())
            {
                var v = x.v;
                lines.Add($"{x.slot,7} | {v.RegistrationNumber,-15} | {v.Colour,-9} | {v.Type,-5} | {v.CheckIn:yyyy-MM-dd HH:mm:ss}");
            }
            if (!OccupiedSlots().Any()) lines.Add("Parking lot is empty.");
            return string.Join(Environment.NewLine, lines);
        }

}

class Program
{
    const decimal DEFAULT_FEE_PER_HOUR = 5000m;

    static void Main()
    {
        ParkingLot? parking = null;

        var commandAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "cpl", "create_parking_lot" },
            { "create", "create_parking_lot" },
            { "pk", "park" },
            { "lv", "leave" },
            { "st", "status" },
            { "tov", "type_of_vehicles" },
            { "rnvc", "registration_numbers_for_vehicles_with_colour" },
            { "snvc", "slot_numbers_for_vehicles_with_colour" },
            { "snr", "slot_number_for_registration_number" },
            { "rno", "registration_numbers_for_vehicles_with_odd_plate" },
            { "rne", "registration_numbers_for_vehicles_with_even_plate" },
            { "h", "help" },
            { "e", "exit" }
        };

        ShowHelpShortHint();

        while (true)
        {
            Console.Write("$ ");
            var line = Console.ReadLine();
            if (line == null) break;
            line = line.Trim();
            if (line.Length == 0) continue;

            var parts = SplitCommand(line);
            var cmd = parts[0].ToLowerInvariant();

            if (commandAliases.ContainsKey(cmd))
            {
                cmd = commandAliases[cmd];
            }

            if (cmd == "exit") break;

            if (cmd == "help")
            {
                ShowHelp();
                continue;
            }

            if (cmd == "create_parking_lot")
            {
                if (parts.Length < 2 || !int.TryParse(parts[1], out int n))
                {
                    Console.WriteLine("Usage: create_parking_lot <number>");
                    continue;
                }
                parking = new ParkingLot(n, DEFAULT_FEE_PER_HOUR);
                Console.WriteLine($"Parking lot diinisialisasi dengan {n} slot.");
                continue;
            }

            if (parking == null)
            {
                Console.WriteLine("Belum ada parking lot. Jalankan: create_parking_lot <n>");
                continue;
            }

            if (cmd == "park")
            {
                if (parts.Length < 4) { Console.WriteLine("Usage: park <registration> <colour> <type>"); continue; }
                string reg = parts[1];
                string colour = parts[2];
                string typeStr = parts[3];

                if (!Enum.TryParse<VehicleType>(typeStr, true, out var type))
                {
                    Console.WriteLine("Tipe kendaraan tidak valid. Gunakan 'Mobil' atau 'Motor'.");
                    continue;
                }

                var v = new Vehicle { RegistrationNumber = reg, Colour = colour, Type = type };
                int result = parking.Park(v);
                if (result == -1) Console.WriteLine("Maaf, tempat parkir penuh.");
                else if (result == -2) Console.WriteLine($"Kendaraan dengan nomor registrasi {reg} sudah terparkir.");
                else Console.WriteLine($"Slot {result} terisi.");
                continue;
            }

            if (cmd == "leave")
            {
                if (parts.Length < 2 || !int.TryParse(parts[1], out int slotNo))
                {
                    Console.WriteLine("Usage: leave <slot_number>");
                    continue;
                }
                var (ok, fee, hours) = parking.Leave(slotNo);
                if (!ok) Console.WriteLine($"Slot {slotNo} tidak valid atau sudah kosong.");
                else
                {
                    Console.WriteLine($"Slot {slotNo} freed. Parking time: {hours} hour(s). Fee: Rp {fee:N0}");
                }
                continue;
            }

            if (cmd == "status")
            {
                Console.WriteLine(parking.Status());
                continue;
            }

            if (cmd == "type_of_vehicles")
            {
                if (parts.Length >= 2)
                {
                    var typeStr = parts[1];
                    if (!Enum.TryParse<VehicleType>(typeStr, true, out var singleType))
                    {
                        Console.WriteLine("Tipe kendaraan tidak valid. Gunakan 'Mobil' atau 'Motor'.");
                    }
                    else
                    {
                        int count = parking.CountByType(singleType);
                        Console.WriteLine($"{singleType}: {count}");
                    }
                }
                else
                {
                    int mobilCount = parking.CountByType(VehicleType.Mobil);
                    int motorCount = parking.CountByType(VehicleType.Motor);
                    Console.WriteLine($"Mobil: {mobilCount}");
                    Console.WriteLine($"Motor: {motorCount}");
                }
                continue;
            }


            if (cmd == "registration_numbers_for_vehicles_with_colour")
            {
                if (parts.Length < 2) { Console.WriteLine("Usage: registration_numbers_for_vehicles_with_colour <colour>"); continue; }
                var colour = parts[1];
                var regs = parking.RegistrationNumbersByColour(colour);
                if (regs.Count == 0) Console.WriteLine("Not found");
                else Console.WriteLine(string.Join(", ", regs));
                continue;
            }

            if (cmd == "slot_numbers_for_vehicles_with_colour")
            {
                if (parts.Length < 2) { Console.WriteLine("Usage: slot_numbers_for_vehicles_with_colour <colour>"); continue; }
                var colour = parts[1];
                var slots = parking.SlotNumbersByColour(colour);
                if (slots.Count == 0) Console.WriteLine("Not found");
                else Console.WriteLine(string.Join(", ", slots));
                continue;
            }

            if (cmd == "slot_number_for_registration_number")
            {
                if (parts.Length < 2) { Console.WriteLine("Usage: slot_number_for_registration_number <registration>"); continue; }
                var reg = parts[1];
                var slot = parking.SlotNumberForRegistration(reg);
                if (slot == null) Console.WriteLine("Not found");
                else Console.WriteLine(slot.Value);
                continue;
            }

            if (cmd == "registration_numbers_for_vehicles_with_odd_plate")
            {
                var regs = parking.RegistrationNumbersByOddEven(true);
                if (regs.Count == 0) Console.WriteLine("Not found");
                else Console.WriteLine(string.Join(", ", regs));
                continue;
            }

            if (cmd == "registration_numbers_for_vehicles_with_even_plate")
            {
                var regs = parking.RegistrationNumbersByOddEven(false);
                if (regs.Count == 0) Console.WriteLine("Not found");
                else Console.WriteLine(string.Join(", ", regs));
                continue;
            }

            Console.WriteLine("Perintah tidak dikenal. Ketik 'help' untuk melihat daftar perintah.");
        }
    }

    static void ShowHelpShortHint()
    {
        Console.WriteLine("Ketikan 'help' atau 'h' untuk daftar perintah. 'e' atau 'exit' untuk keluar.");
    }

    static void ShowHelp()
    {
        var commands = new List<(string Command, string Description)>
        {
            ("create_parking_lot <n> (alias: cpl, create)", "Buat parking lot dengan <n> slot."),
            ("park <registration> <colour> <type> (alias: pk)", "Parkirkan kendaraan. type: Mobil atau Motor."),
            ("leave <slot_number> (alias: lv)", "Keluarkan kendaraan dari slot."),
            ("status (alias: st)", "Tampilkan status saat ini."),
            ("type_of_vehicles (alias: tov)", "Tampilkan jumlah kendaraan per tipe."),
            ("registration_numbers_for_vehicles_with_colour <colour> (alias: rnvc)", "Nomor registrasi kendaraan berdasarkan warna."),
            ("slot_numbers_for_vehicles_with_colour <colour> (alias: snvc)", "Nomor slot kendaraan berdasarkan warna."),
            ("slot_number_for_registration_number <registration> (alias: snr)", "Nomor slot untuk sebuah registrasi."),
            ("registration_numbers_for_vehicles_with_odd_plate (alias: rno)", "Nomor registrasi dengan digit akhir ganjil."),
            ("registration_numbers_for_vehicles_with_even_plate (alias: rne)", "Nomor registrasi dengan digit akhir genap."),
            ("help (alias: h)", "Tampilkan help ini."),
            ("exit (alias: e)", "Keluar dari aplikasi.")
        };

        Console.WriteLine("Available Commands:");
        Console.WriteLine(new string('-', 80));
        foreach (var cmd in commands)
        {
            Console.WriteLine($"{cmd.Command,-55} - {cmd.Description}");
        }
        Console.WriteLine(new string('-', 80));
    }

    static string[] SplitCommand(string input) =>
        input.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
}
