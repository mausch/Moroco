using System;
using System.Linq;
using System.Threading;

namespace Moroco {
    public class MorocoException : Exception {
        public MorocoException(string message) : base(message) {}
        public MorocoException(string message, Exception innerException) : base(message, innerException) {}
    }

    public abstract class Unit {}

    public abstract class Counter {
        public readonly long? Min;
        public readonly long? Max;

        protected Counter(long expected) {
            Min = Max = expected;
        }

        protected Counter(long? min = null, long? max = null) {
            Min = min;
            Max = max;
        }

        private long count;

        public long Calls {
            get { return count; }
        }

        protected long Incr() {
            var r = Interlocked.Increment(ref count);
            if (Max.HasValue && r > Max)
                throw new MorocoException(string.Format("Called more than {0} times", Max));
            return r;
        }

        public void Verify() {
            if (Max.HasValue && Min.HasValue && Min == Max && count != Min)
                throw new MorocoException(string.Format("Expected {0} calls, actual {1}", Min, count));
            if (Min.HasValue & count < Min)
                throw new MorocoException(string.Format("Expected at least {0} calls, actual {1}", Min, count));
            if (Max.HasValue && count > Max)
                throw new MorocoException(string.Format("Called more than {0} times", Max));
        }
    }

    public class MFunc<A>: Counter {
        private static readonly Func<A> def = () => default(A);
        private readonly Func<A> fun = def;

        public MFunc(long expected): base(expected) {}

        public MFunc(long? min = null, long? max = null): base(min, max) {}

        public MFunc(Func<A> f, long expected) : this(expected) {
            fun = f ?? def;
        }

        public MFunc(Func<A> f, long? min = null, long? max = null) : this(min, max) {
            fun = f ?? def;
        }

        public A Invoke() {
            Incr();
            return fun();
        }

        public static implicit operator MFunc<A>(Func<A> f) {
            return new MFunc<A>(f);
        }

        public static implicit operator Func<A>(MFunc<A> f) {
            return f.Invoke;
        }

        public static MFunc<A> operator +(MFunc<A> a, Func<A> b) {
            a = a ?? def;
            return new MFunc<A>(a.fun + b, a.Min, a.Max);
        }
    }

    public class MFunc<A, B>: Counter {
        private static readonly Func<A, B> def = a => default(B);
        private readonly Func<A, B> fun = def;

        public MFunc(long expected): base(expected) {}

        public MFunc(long? min = null, long? max = null): base(min, max) {}

        public MFunc(Func<A, B> f, long expected) : this(expected) {
            fun = f ?? def;
        }

        public MFunc(Func<A, B> f, long? min = null, long? max = null) : this(min, max) {
            fun = f ?? def;
        }

        public B Invoke(A a) {
            Incr();
            return fun(a);
        }

        public static implicit operator MFunc<A, B>(Func<A, B> f) {
            return new MFunc<A, B>(f);
        }

        public static implicit operator Func<A, B>(MFunc<A, B> f) {
            return f.Invoke;
        }

        public static MFunc<A, B> operator +(MFunc<A, B> a, Func<A, B> b) {
            a = a ?? def;
            return new MFunc<A, B>(a.fun + b, a.Min, a.Max);
        }

        public static MFunc<A, B> operator &(MFunc<A, B> a, Func<MFunc<A, B>, MFunc<A, B>> map) {
            return map(a);
        }

    }

    public class MFunc<A, B, C>: Counter {
        private static readonly Func<A, B, C> def = (a, b) => default(C);
        private readonly Func<A, B, C> fun = def;

        public MFunc(long expected): base(expected) {}

        public MFunc(long? min = null, long? max = null): base(min, max) {}

        public MFunc(Func<A, B, C> f, long expected) : this(expected) {
            fun = f ?? def;
        }

        public MFunc(Func<A, B, C> f, long? min = null, long? max = null) : this(min, max) {
            fun = f ?? def;
        }

        public C Invoke(A a, B b) {
            Incr();
            return fun(a, b);
        }

        public static implicit operator MFunc<A, B, C>(Func<A, B, C> f) {
            return new MFunc<A, B, C>(f);
        }

        public static implicit operator Func<A, B, C>(MFunc<A, B, C> f) {
            return f.Invoke;
        }

        public static MFunc<A, B, C> operator +(MFunc<A, B, C> a, Func<A, B, C> b) {
            a = a ?? def;
            return new MFunc<A, B, C>(a.fun + b, a.Min, a.Max);
        }

        public static MFunc<A,B,C> operator &(MFunc<A,B,C> a, Func<MFunc<A,B,C>, MFunc<A,B,C>> map) {
            return map(a);
        }
    }

    public class MFunc<A, B, C, D>: Counter {
        private static readonly Func<A, B, C, D> def = (a, b, c) => default(D);
        private readonly Func<A, B, C, D> fun = def;

        public MFunc(long expected): base(expected) {}

        public MFunc(long? min = null, long? max = null): base(min, max) {}

        public MFunc(Func<A, B, C, D> f, long expected): this(expected) {
            fun = f ?? def;
        }

        public MFunc(Func<A, B, C, D> f, long? min = null, long? max = null): this(min, max) {
            fun = f ?? def;
        }

        public D Invoke(A a, B b, C c) {
            Incr();
            return fun(a, b, c);
        }

        public static implicit operator MFunc<A, B, C, D>(Func<A, B, C, D> f) {
            return new MFunc<A, B, C, D>(f);
        }

        public static implicit operator Func<A, B, C, D>(MFunc<A, B, C, D> f) {
            return f.Invoke;
        }

        public static MFunc<A, B, C, D> operator +(MFunc<A, B, C, D> a, Func<A, B, C, D> b) {
            a = a ?? def;
            return new MFunc<A, B, C, D>(a.fun + b, a.Min, a.Max);
        }
    }

    public class MFunc<A, B, C, D, R>: Counter {
        private static readonly Func<A, B, C, D, R> def = (a, b, c, d) => default(R);
        private readonly Func<A, B, C, D, R> fun = def;

        public MFunc(long expected): base(expected) {}

        public MFunc(long? min = null, long? max = null): base(min, max) {}

        public MFunc(Func<A, B, C, D, R> f, long expected): this(expected) {
            fun = f ?? def;
        }

        public MFunc(Func<A, B, C, D, R> f, long? min = null, long? max = null): this(min, max) {
            fun = f ?? def;
        }

        public R Invoke(A a, B b, C c, D d) {
            Incr();
            return fun(a, b, c, d);
        }

        public static implicit operator MFunc<A, B, C, D, R>(Func<A, B, C, D, R> f) {
            return new MFunc<A, B, C, D, R>(f);
        }

        public static implicit operator Func<A, B, C, D, R>(MFunc<A, B, C, D, R> f) {
            return f.Invoke;
        }

        public static MFunc<A, B, C, D, R> operator +(MFunc<A, B, C, D, R> a, Func<A, B, C, D, R> b) {
            a = a ?? def;
            return new MFunc<A, B, C, D, R>(a.fun + b, a.Min, a.Max);
        }
    }


    public abstract class Validation {
        public abstract T Match<T>(Func<T> ok, Func<string, T> fail);
    }

    public class Ok : Validation {
        public override T Match<T>(Func<T> ok, Func<string, T> fail) {
            return ok();
        }
    }

    public class Fail : Validation {
        private readonly string message;

        public Fail(string message) {
            this.message = message;
        }

        public override T Match<T>(Func<T> ok, Func<string, T> fail) {
            return fail(message);
        }
    }

    public static class M {
        public static void VerifyAll(this object o) {
            if (o == null)
                return;
            var counters =
                o.GetType().GetFields()
                    .Where(x => typeof(Counter).IsAssignableFrom(x.FieldType))
                    .Select(x => new { x.Name, Counter = (Counter)x.GetValue(o) })
                    .Where(x => x.Counter != null);
            foreach (var c in counters)
                try {
                    c.Counter.Verify();
                } catch (MorocoException e) {
                    throw new MorocoException(c.Name + " failed", e);
                }            
        }

        public static Ok Ok() {
            return new Ok();
        }

        public static Fail Fail(string message) {
            return new Fail(message);
        }

        public static Func<Unit> ToFunc(this Action action) {
            return () => {
                action();
                return null;
            };
        }

        public static Func<A, Unit> ToFunc<A>(this Action<A> action) {
            return a => {
                action(a);
                return null;
            };
        }

        public static Func<A, B, Unit> ToFunc<A, B>(this Action<A, B> action) {
            return (a, b) => {
                action(a, b);
                return null;
            };
        }

        public static Action<A, B> ToAction<A, B, C>(this Func<A, B, C> f) {
            return (a, b) => f(a, b);
        }

        public static MFunc<A, B, C> Arg1<A, B, C>(this MFunc<A, B, C> f, Func<A, Validation> validate) {
            f = f ?? new MFunc<A, B, C>();
            return new MFunc<A, B, C>((a, b) => validate(a)
                                                          .Match<C>(ok : () => f.Invoke(a, b),
                                                                 fail : msg => {
                                                                     throw new MorocoException(msg);
                                                                 }), f.Min, f.Max);
        }

        public static MFunc<A, B, C> Arg1<A, B, C>(this MFunc<A, B, C> f, A value) {
            return Arg1(f, x => {
                if (Equals(x, value))
                    return Ok();
                return Fail(string.Format("Invalid first argument, expected {0}, was {1}", value, x));
            });
        }

        public static MFunc<A, B, C> Arg2<A, B, C>(this MFunc<A, B, C> f, Func<B, Validation> validate) {
            f = f ?? new MFunc<A, B, C>();
            return new MFunc<A, B, C>((a, b) => validate(b)
                                                          .Match<C>(ok : () => f.Invoke(a, b),
                                                                 fail : msg => {
                                                                     throw new MorocoException(msg);
                                                                 }), f.Min, f.Max);
        }

        public static MFunc<A, B, C> Arg2<A, B, C>(this MFunc<A, B, C> f, B value) {
            return Arg2(f, x => {
                if (Equals(x, value))
                    return Ok();
                return Fail(string.Format("Invalid second argument, expected {0}, was {1}", value, x));
            });
        }

        public static MFunc<A, B, C> Args<A, B, C>(this MFunc<A, B, C> f, A a, B b) {
            return f.Arg1(a).Arg2(b);
        }

        public static MFunc<A, B> Return<A, B>(this MFunc<A, B> f, B value) {
            return f + (_ => value);
        }

        public static MFunc<A, B, C> Return<A, B, C>(this MFunc<A, B, C> f, C value) {
            return f + ((a, b) => value);
        }

        public static MFunc<A> Expect<A>(this MFunc<A> f, long expected) {
            f = f ?? new MFunc<A>();
            return new MFunc<A>(f, expected);
        }

        public static MFunc<A, B> Expect<A, B>(this MFunc<A, B> f, long expected) {
            f = f ?? new MFunc<A, B>();
            return new MFunc<A, B>(f, expected);
        }

        public static MFunc<A, B, C> Expect<A, B, C>(this MFunc<A, B, C> f, long expected) {
            f = f ?? new MFunc<A, B, C>();
            return new MFunc<A, B, C>(f, expected);
        }

        public static MFunc<A, B> Stub<A, B>(this MFunc<A, B> f) {
            return new MFunc<A, B>();
        }

        public static MFunc<A,B,C> Stub<A,B,C>(this MFunc<A,B,C> f) {
            return new MFunc<A, B, C>();
        }
    }
}