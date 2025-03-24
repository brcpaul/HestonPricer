using HestonPricer.Models;

public abstract class PricerBase {

    protected OptionBase option;
    public PricerBase(OptionBase option) {
        this.option = option;
    }

    public OptionBase Option {
        get { return option; }
    }

    public abstract double Price();
} 